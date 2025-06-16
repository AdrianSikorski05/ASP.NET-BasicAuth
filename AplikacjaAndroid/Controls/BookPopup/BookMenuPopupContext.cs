using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;

namespace AplikacjaAndroid
{
    public partial class BookMenuPopupContext(ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage) : ObservableObject
    {
        private Book _book;

        private SnackbarOptions _succesSnackBarOption = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#059669"),
            ActionButtonTextColor = Colors.White,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            TextColor = Colors.White,
        };

        private SnackbarOptions _errorSnackBarOption = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#f43f5e"),
            ActionButtonTextColor = Colors.White,
            CornerRadius = 8,
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            TextColor = Colors.White
        };

        [ObservableProperty] private bool _isDeleteButtonVisible = true;

        [RelayCommand]
        public async Task MarkAsRead()
        {
            string message = "";
            string route = "//readed";
            SnackbarOptions snackBar;

            if (readedBookStorage.IfBookExists(_book))
            {
                snackBar = _errorSnackBarOption;
                message = "The book is already added to your read list.";
            }
            else if (toReadBookStorage.IfBookExists(_book))
            {
                snackBar = _succesSnackBarOption;
                toReadBookStorage.Remove(_book);
                readedBookStorage.Add(_book);
                message = "The book has been moved to read.";
            }
            else
            {
                snackBar = _succesSnackBarOption;
                readedBookStorage.Add(_book);
                message = "The book added to read.";
            }

            await MopupService.Instance.PopAllAsync();

            await Snackbar.Make(message, action: async () =>
            {
                // reset stosu w aktualnej zakładce
                await Shell.Current.Navigation.PopToRootAsync();

                // przejście do zakładki "Readed" bez historii
                await Shell.Current.GoToAsync(route);

            }, actionButtonText: "Show", visualOptions: snackBar).Show();
        }

        [RelayCommand]
        public async Task DeleteBook()
        {
            string message = "";
            SnackbarOptions snackBar;

            if (toReadBookStorage.IfBookExists(_book))
            {               
                toReadBookStorage.Remove(_book);
                message = "Removed from books to read";
            }
            else if (readedBookStorage.IfBookExists(_book))
            {                
                readedBookStorage.Remove(_book);
                message = "Removed from books read.";
            }
            else
            {

            }
                snackBar = _succesSnackBarOption;

            await MopupService.Instance.PopAllAsync();

            await Snackbar.Make(message, action: async () =>
            {
                // reset stosu w aktualnej zakładce
                await Shell.Current.Navigation.PopToRootAsync();

            }, visualOptions: snackBar).Show();
        }

        [RelayCommand]
        public async Task MarkAsToRead()
        {
            string message = "";
            string route = "//toRead";
            SnackbarOptions snackBar;

            if (toReadBookStorage.IfBookExists(_book)) 
            {
                snackBar = _errorSnackBarOption;
                message = "The book is already marked as ready to read.";
            }
            else if (readedBookStorage.IfBookExists(_book))
            {
                snackBar = _succesSnackBarOption;
                readedBookStorage.Remove(_book);
                toReadBookStorage.Add(_book);
                message = "The book has been moved from read to to read.";
            }
            else
            {
                snackBar = _succesSnackBarOption;
                toReadBookStorage.Add(_book);
                message = "The book has been marked as to read.";
            }

            await MopupService.Instance.PopAllAsync();

            await Snackbar.Make(message, action: async () =>
            {
                // reset stosu w aktualnej zakładce
                await Shell.Current.Navigation.PopToRootAsync();

                // przejście do zakładki "Readed" bez historii
                await Shell.Current.GoToAsync(route);

            }, actionButtonText: "Show", visualOptions: snackBar).Show();
        }

        [RelayCommand]
        public async Task Close()
        {
            await MopupService.Instance.PopAllAsync();
        }

        public void LoadContext(Book book, bool isDeleteButtonVisible)
        {
            _book = book;
            IsDeleteButtonVisible = isDeleteButtonVisible;
        }
    }
}
