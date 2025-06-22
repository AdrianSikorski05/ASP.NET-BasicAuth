using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public class NavigationService
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool _isNavigating = false;

        public async Task NavigateToAsync(string route, Dictionary<string, object>? parameters = null)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (_isNavigating) return;
                _isNavigating = true;

                await Task.Delay(50); // małe opóźnienie na bezpieczne przejście

                if (parameters != null)
                    await Shell.Current.GoToAsync(route, true, parameters);
                else
                    await Shell.Current.GoToAsync(route);

                await Task.Delay(100); // zabezpieczenie przed spamowaniem
            }
            finally
            {
                _isNavigating = false;
                _semaphore.Release();
            }
        }

        public async Task GoToRootAsync(string route)
        {
            await _semaphore.WaitAsync();

            try
            {
                if (_isNavigating) return;
                _isNavigating = true;

                await Shell.Current.Navigation.PopToRootAsync();
                await Task.Delay(50);
                await Shell.Current.GoToAsync(route);
                await Task.Delay(100);
            }
            finally
            {
                _isNavigating = false;
                _semaphore.Release();
            }
        }
    }
}
