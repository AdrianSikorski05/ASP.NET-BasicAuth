using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AplikacjaAndroid
{
    public partial class DetailsBookContext(ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage) : ObservableObject
    {
        [ObservableProperty]
        private Book _book;

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

            if (toReadBookStorage.IfBookExists(Book))
            {
                message = "The book is already added to read.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f43f5e");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                stackbarOption.TextColor = Colors.White;

            }
            else if (readedBookStorage.IfBookExists(Book))
            {
                message = @"The book is already marked as read.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f97316");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                route = "//readed";
            }
            else
            {
                message = "Book added to read.";
                toReadBookStorage.Add(Book);
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

            if (readedBookStorage.IfBookExists(Book))
            {
                message = "The book is already added to your read list.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f43f5e");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                stackbarOption.TextColor = Colors.White;
            }
            else if (toReadBookStorage.IfBookExists(Book))
            {
                message = "The book is already placed for reading.";
                stackbarOption.BackgroundColor = Color.FromArgb("#f97316");
                stackbarOption.ActionButtonTextColor = Colors.Black;
                route = "//toRead";
            }
            else
            {
                readedBookStorage.Add(Book);
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
    }
}
