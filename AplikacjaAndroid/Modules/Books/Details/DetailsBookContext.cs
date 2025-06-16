using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    public partial class DetailsBookContext : ObservableObject
    {     
        [ObservableProperty]
        private Book _book;

        [ObservableProperty]
        bool _isVisibleButtons;

        [ObservableProperty]
        bool _isVisibleConfig;

        [ObservableProperty]
        string _commentText;

        private readonly ReadedBookStorage _readedBookStorage;       
        private readonly ToReadBookStorage _toReadBookStorage;
        private readonly BookMenuPopup _bookMenuPopup;

        [ObservableProperty]
        public List<int> _ratingOptions = new List<int>{ 1, 2, 3, 4, 5 };

        public DetailsBookContext(ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage, BookMenuPopup bookMenuPopup)
        {           
            _readedBookStorage = readedBookStorage;
            _toReadBookStorage = toReadBookStorage;
            _bookMenuPopup = bookMenuPopup;
            Comments = new ObservableCollection<CommentBook> 
            { 
                new CommentBook {id = 1, Author = "Admin",Content = "testowy komentarz",Rate = 1,PublishedDate = DateTime.Now},
                new CommentBook {id = 2, Author = "Kutas",Content = "testowy komentarz1",Rate = 4,PublishedDate = DateTime.Now},
                new CommentBook {id = 3, Author = "Pizda",Content = "testowy komentarz testowy komentarz testowy komentarz testowy komentarz testowy komentarz testowy komentarz",Rate = 5,PublishedDate = DateTime.Now},
                new CommentBook {id = 4, Author = "Dupa",Content = "testowy komentarz2",Rate = 3,PublishedDate = DateTime.Now},
                new CommentBook {id = 5, Author = "Chuj",Content = "testowy komentarz3",Rate = 5,PublishedDate = DateTime.Now},
            };
        }

        [ObservableProperty]
        public ObservableCollection<CommentBook> _comments = new ();

        [RelayCommand]
        public async Task AddToToReadCollection()
        {
            var stackbarOption = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#a78bfa"),
                ActionButtonTextColor = Colors.Red,
                CornerRadius = 8,
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                TextColor = Colors.Black,
                
            };

            string message = "";
            string route = "//toRead";

            if (_toReadBookStorage.IfBookExists(Book))
            {
                message = "The book is already added to read.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f43f5e");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                stackbarOption.TextColor = Colors.White;

            }
            else if (_readedBookStorage.IfBookExists(Book))
            {
                message = @"The book is already marked as read.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f97316");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                route = "//readed";
            }
            else
            {
                message = "Book added to read.";
                _toReadBookStorage.Add(Book);
            }

            await Snackbar.Make(message, action: async () =>
            {
                // reset stosu w aktualnej zakładce
                await Shell.Current.Navigation.PopToRootAsync();

                // przejście do zakładki "toRead" bez historii
                await Shell.Current.GoToAsync(route);
            }, actionButtonText: "Show", visualOptions: stackbarOption).Show();
        }

        [RelayCommand]
        public async Task AddToReadedCollection()
        {
            var stackbarOption = new SnackbarOptions
            {
                BackgroundColor = Color.FromArgb("#34d399"),
                ActionButtonTextColor = Colors.Red,
                CornerRadius = 8,
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                TextColor = Colors.Black,
                
            };

            string message = "";
            string route = "//readed";

            if (_readedBookStorage.IfBookExists(Book))
            {
                message = "The book is already added to your read list.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f43f5e");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                stackbarOption.TextColor = Colors.White;
            }
            else if (_toReadBookStorage.IfBookExists(Book))
            {
                message = "The book is already placed for reading.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f97316");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                route = "//toRead";
            }
            else
            {
                _readedBookStorage.Add(Book);
                message = "Book added to read list.";
            }


            await Snackbar.Make(message, action: async () =>
            {
                // reset stosu w aktualnej zakładce
                await Shell.Current.Navigation.PopToRootAsync();

                // przejście do zakładki "Readed" bez historii
                await Shell.Current.GoToAsync(route);

            }, actionButtonText: "Show", visualOptions: stackbarOption).Show();
        }

        [RelayCommand]
        public async Task GoToDetailsView(Book selectedBook)
        {
            await Shell.Current.GoToAsync(nameof(DetailsBookView), true, new Dictionary<string, object>
        {
            { "Book", selectedBook }
        });
        }

        [RelayCommand]
        public async Task ShowPopup()
        {
            _bookMenuPopup.LoadContext(_book, true);
            await MopupService.Instance.PushAsync(_bookMenuPopup, true);
        }
       
        [RelayCommand]
        void ClearComment()
        {
            CommentText = string.Empty;
        }
    }
}
