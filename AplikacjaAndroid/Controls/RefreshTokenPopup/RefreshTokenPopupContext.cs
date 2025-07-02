using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace AplikacjaAndroid
{
    public partial class RefreshTokenPopupContext : ObservableObject
    {
        private TaskCompletionSource<bool> _completionSource;

        public void SetCompletionSource(TaskCompletionSource<bool> tcs)
        {
            _completionSource = tcs;
        }



        [RelayCommand]
        public async Task Close()
        {
            _completionSource?.SetResult(false);
            await MopupService.Instance.PopAllAsync();
        }


        [RelayCommand]
        public async Task Extend() 
        {
            _completionSource?.SetResult(true);
            await MopupService.Instance.PopAllAsync();
        }
    }
}
