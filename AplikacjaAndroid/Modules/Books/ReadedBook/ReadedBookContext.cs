using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    public partial class ReadedBookContext : ObservableObject
    {
        private readonly BookMenuPopup _bookMenuPopup;

        public ReadedBookContext(ReadedBookStorage readedBookStorage, BookMenuPopup bookMenuPopup)
        {
            _readedBooks = readedBookStorage.Books;
            _bookMenuPopup = bookMenuPopup;
        }

        [ObservableProperty]
        private ObservableCollection<Book> _readedBooks;


        [RelayCommand]
        public async Task GoToDetailsView(Book selectedBook)
        {
            await Shell.Current.GoToAsync(nameof(DetailsBookView), true, new Dictionary<string, object>
        {
            { "Book", selectedBook },
            { "IsVisibleButtons", false },
            { "IsVisibleConfig", true }
        });
        }

        [RelayCommand]
        public async Task ShowPopup(Book book)
        {
            _bookMenuPopup.LoadContext(book, true);
            await MopupService.Instance.PushAsync(_bookMenuPopup, true);
        }       
    }
}
