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
                await toReadBookStorage.Remove(_book);
                await readedBookStorage.Add(_book);
                message = "The book has been moved to read.";
            }
            else
            {
                snackBar = _succesSnackBarOption;
                await readedBookStorage.Add(_book);
                message = "The book added to read.";
            }

            await MopupService.Instance.PopAllAsync();

            await Snackbar.Make(message, action: async () =>
            {
                await Shell.Current.Navigation.PopToRootAsync();
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
                await toReadBookStorage.Remove(_book);
                message = "Removed from books to read";
            }
            else if (readedBookStorage.IfBookExists(_book))
            {
                await readedBookStorage.Remove(_book);
                message = "Removed from books read.";
            }
            else
            {

            }
            snackBar = _succesSnackBarOption;

            await Close();

            await Snackbar.Make(message, action: async () =>
            {
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
                await readedBookStorage.Remove(_book);
                await toReadBookStorage.Add(_book);
                message = "The book has been moved from read to to read.";
            }
            else
            {
                snackBar = _succesSnackBarOption;
                await toReadBookStorage.Add(_book);
                message = "The book has been marked as to read.";
            }

            await Close();

            await Snackbar.Make(message, action: async () =>
            {
                await Shell.Current.Navigation.PopToRootAsync();
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
