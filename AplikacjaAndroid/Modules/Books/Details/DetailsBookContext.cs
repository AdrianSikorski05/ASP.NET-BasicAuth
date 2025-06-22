using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    [QueryProperty(nameof(IsVisibleButtons), "IsVisibleButtons")]
    [QueryProperty(nameof(IsVisibleConfig), "IsVisibleConfig")]
    [QueryProperty(nameof(Book), "Book")]
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

        [ObservableProperty]
        bool _isLoading = false;

        [ObservableProperty]
        bool _areCommentsLoaded = false;

        private CommentFilter _commentFilter = new();

        private readonly ReadedBookStorage _readedBookStorage;
        private readonly ToReadBookStorage _toReadBookStorage;
        private readonly BookMenuPopup _bookMenuPopup;
        private readonly UserStorage _userStorage;
        private readonly ICommentService _commentService;
        private readonly CommentPopupView _commentPopupView;
        private readonly NavigationService _navigationService;
        private readonly IServiceProvider _serviceProvider;


        public DetailsBookContext(ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage
            , BookMenuPopup bookMenuPopup, UserStorage userStorage, ICommentService commentService,
            CommentPopupView commentPopupView, NavigationService navigationService , IServiceProvider serviceProvider)
        {
            _readedBookStorage = readedBookStorage;
            _toReadBookStorage = toReadBookStorage;
            _bookMenuPopup = bookMenuPopup;
            _userStorage = userStorage;
            _commentService = commentService;
            _commentPopupView = commentPopupView;
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
        }

        [ObservableProperty]
        public ObservableCollection<CommentBook> _comments = new();

        [ObservableProperty]
        public CreateCommentBookDto _newComment = new();

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
                await _toReadBookStorage.Add(Book);
            }

            await Snackbar.Make(message, action: async () =>
            {
                await Shell.Current.Navigation.PopToRootAsync();
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
                await _readedBookStorage.Add(Book);
                message = "Book added to read list.";
            }


            await Snackbar.Make(message, action: async () =>
            {
                await _navigationService.GoToRootAsync(route);

            }, actionButtonText: "Show", visualOptions: stackbarOption).Show();
        }

        [RelayCommand]
        public async Task GoToDetailsView(Book selectedBook)
        {
            await _navigationService.NavigateToAsync(nameof(DetailsBookView), new()
            {
                { "Book", selectedBook }
            });
        }

        [RelayCommand]
        public async Task ShowPopup()
        {
            var context = _bookMenuPopup.BindingContext as BookMenuPopupContext;
            context?.LoadContext(Book, false);
            await MopupService.Instance.PushAsync(_bookMenuPopup, true);

        }

        [RelayCommand]
        void ClearComment()
        {
            NewComment.Content = string.Empty;
            NewComment.Rate = 0;
        }

        [RelayCommand]
        public async Task AddComment()
        {
            if (NewComment.IsValid())
            {
                NewComment.BookId = Book.Id;
                NewComment.UserId = _userStorage.User.Id;
                NewComment.Author = _userStorage.User.Username;
                var result = await _commentService.AddCommentAsync(NewComment);
                if (result != null && result.StatusCode == 200 && result.Data != null)
                {
                    Comments.Insert(0, result.Data);
                    NewComment = new();
                }
            }
        }

        [ObservableProperty]
        bool _hasMorePages;

        [RelayCommand]
        public async Task LoadComments()
        {
            if (AreCommentsLoaded)
                return;

            IsLoading = true;
            _commentFilter.Page = 1;

            _commentFilter.BookId = Book.Id;
            var response = await _commentService.GetComments(_commentFilter);

            if (response != null && response.StatusCode == 200 && response.Data?.Items != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Comments = new ObservableCollection<CommentBook>(response.Data.Items);                    
                });

                response.Data.Items.Where(c => c.UserId == _userStorage?.User?.Id).ToList().ForEach(c => c.IsOwner = true);

                HasMorePages = response.Data.TotalPages >= _commentFilter.Page;
                AreCommentsLoaded = true;
            }
            else
            {
                Comments = new ObservableCollection<CommentBook>();
                HasMorePages = false;
            }

            IsLoading = false;
        }

        [ObservableProperty]
        bool _isLoadingNextComment = false;

        [RelayCommand]
        public async Task LoadNextPageComments()
        {
            if (!HasMorePages)
                return;

            IsLoadingNextComment = true;
            _commentFilter.Page++;

            var result = await _commentService.GetComments(_commentFilter);

            if (result?.Data?.Items != null && result.Data.Items.Any())
            {
                foreach (var item in result.Data.Items)
                {                   
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Comments.Add(item);
                    });

                    foreach (var comment in Comments)
                    {
                        comment.IsOwner = comment.UserId == _userStorage?.User?.Id;
                    }

                }

                HasMorePages = result.Data.TotalPages >= _commentFilter.Page;
            }
            else
            {
                HasMorePages = false;
            }

            IsLoadingNextComment = false;
        }

        [RelayCommand]
        public async Task OpenCommentPopup(CommentBook comment)
        {
            var context = _serviceProvider.GetRequiredService<CommentPopupContext>();
            context.LoadContext(comment);

            var popup = new CommentPopupView(context); // nowa instancja popupu
            await MopupService.Instance.PushAsync(popup, true);

            var result = await context.ShowAsync();
            if (result != null)
            {
                var original = Comments.FirstOrDefault(c => c.Id == result.Id);
                if (original != null)
                {
                    var index = Comments.IndexOf(original);
        Comments.RemoveAt(index);
        Comments.Insert(index, result);
                }
            }
            await Task.Delay(1000);
        }
    }
}
