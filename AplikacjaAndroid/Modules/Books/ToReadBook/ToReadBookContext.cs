using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    public partial class ToReadBookContext : ObservableObject
    {
        private readonly BookMenuPopup _bookMenuPopup;
        private readonly NavigationService _navigationService;

        public ToReadBookContext(NavigationService navigationService, ToReadBookStorage toReadBookStorage, BookMenuPopup bookMenuPopup)
        {
            _toReadBooks = toReadBookStorage.Books;
            _bookMenuPopup = bookMenuPopup;
            _navigationService = navigationService;
        }

        [ObservableProperty]
        private ObservableCollection<Book> _toReadBooks;

        private bool _isNavigating = false;

        [RelayCommand]
        public async Task GoToDetailsView(Book selectedBook)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            try
            {
                await _navigationService.NavigateToAsync(nameof(DetailsBookView), new()
                {
                    { "Book", selectedBook },
                    { "IsVisibleButtons", false },
                    { "IsVisibleConfig", true }
                });
            }
            finally
            {
                _isNavigating = false;
            }
        }

        private bool _isPopupOpen = false;

        [RelayCommand]
        public async Task ShowPopup(Book book)
        {
            if (_isPopupOpen) return;
            _isPopupOpen = true;

            try
            {
                var context = _bookMenuPopup.BindingContext as BookMenuPopupContext;
                context?.LoadContext(book, true);
                await MopupService.Instance.PushAsync(_bookMenuPopup, true);
            }
            finally
            {
                _isPopupOpen = false;
            }
        }
    }
}
