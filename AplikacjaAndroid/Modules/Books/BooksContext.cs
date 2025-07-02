using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AplikacjaAndroid;

public partial class BooksContext : ObservableObject
{
    private readonly IBooksService _booksService;
    private readonly BookMenuPopup _bookMenuPopup;
    private readonly ReadedBookStorage _readedBookStorage;
    private readonly ToReadBookStorage _toReadBookStorage;
    private readonly NavigationService _navigationService;



    private List<Book> _allLoadedBooks = new List<Book>();

    public BooksContext(IBooksService booksService, BookMenuPopup bookMenuPopup, ReadedBookStorage readedBookStorage
        , ToReadBookStorage toReadBookStorage, NavigationService navigationService)
    {
        _booksService = booksService;
        _bookMenuPopup = bookMenuPopup;
        _readedBookStorage = readedBookStorage;
        _toReadBookStorage = toReadBookStorage;
        _navigationService = navigationService;
        _ = LoadBooksFromServerAsync();
    }

    [ObservableProperty]
    BookFilter _bookFilter = new();

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    ObservableCollection<string> _availableGenre = new ObservableCollection<string>() { "All", "test", "kupa", "test11111", "111" };


    [ObservableProperty]
    ObservableCollection<int?> _pageSizeSource = new() { 10, 20, 30 };

    [ObservableProperty]
    int? _selectedPageSize = 10;
    partial void OnSelectedPageSizeChanged(int? value)
    {
        BookFilter.PageSize = value;
        _ = LoadBooksFromServerAsync();
    }

    [ObservableProperty]
    ObservableCollection<Book> _books = new ObservableCollection<Book>();

    [ObservableProperty]
    string? _selectedGenre = "All";
    partial void OnSelectedGenreChanged(string? value)
    {
        BookFilter.Genre = value == "All" ? null : value;
        _ = LoadBooksFromServerAsync();
    }

    [ObservableProperty]
    string? _titleFilter;
    partial void OnTitleFilterChanged(string? value)
    {
        BookFilter.Title = value;
        DebouncedReloadFromServer();
    }

    [ObservableProperty]
    string? _authorFilter;
    partial void OnAuthorFilterChanged(string? value)
    {
        BookFilter.Author = value;
        DebouncedReloadFromServer();
    }

    private CancellationTokenSource? _debounceTokenSource;
    private void DebouncedReloadFromServer()
    {
        _debounceTokenSource?.Cancel();
        _debounceTokenSource = new CancellationTokenSource();
        var token = _debounceTokenSource.Token;

        Task.Run(async () =>
        {
            try
            {
                await Task.Delay(1200, token);
                await LoadBooksFromServerAsync();
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during debounced server reload: {ex.Message}");
            }
        });
    }

    /// <summary>
    /// G³ówna metoda do ³adowania ksi¹¿ek z serwera z uwzglêdnieniem bie¿¹cych filtrów.
    /// Zawsze resetuje paginacjê do strony 1.
    /// </summary>
    private async Task LoadBooksFromServerAsync()
    {
        IsLoading = true;
        BookFilter.Page = 1;

        var response = await _booksService.GetAllBooks(BookFilter);

        if (response != null && response.StatusCode == 200 && response.Data?.Items != null)
        {
            _allLoadedBooks = response.Data.Items.ToList();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Books = new ObservableCollection<Book>(_allLoadedBooks);
            });
            SetBookStatuses(Books);
        }
        else
        {
            _allLoadedBooks = new List<Book>();
            Books = new ObservableCollection<Book>();
        }
        IsLoading = false;
    }

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
                { "IsVisibleButtons", true },
                { "IsVisibleConfig", false }
            });
        }
        finally
        {
            _isNavigating = false;
        }
    }

    [RelayCommand]
    public async Task LoadNextPage()
    {
        if (Books.Count < BookFilter.PageSize)
            return;

        IsLoading = true;
        BookFilter.Page++;

        var result = await _booksService.GetAllBooks(BookFilter);

        if (result?.Data?.Items != null && result.Data.Items.Any())
        {
            SetBookStatuses(result.Data.Items);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                foreach (var item in result.Data.Items)
                {
                    Books.Add(item);
                }
            });
            _allLoadedBooks.AddRange(result.Data.Items);
        }
        IsLoading = false;
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
            context?.LoadContext(book, false);
            await MopupService.Instance.PushAsync(_bookMenuPopup, true);

            await Task.Delay(700); // to mo¿e byæ w¹tpliwe UX, ale ok
            SetBookStatuses(Books);
        }
        finally
        {
            _isPopupOpen = false;
        }

    }

    public void SetBookStatuses(IEnumerable<Book> books)
    {
        var readedIds = _readedBookStorage.Books.Select(b => b.Id).ToHashSet();
        var toReadIds = _toReadBookStorage.Books.Select(b => b.Id).ToHashSet();

        foreach (var book in books)
        {
            book.UpdateStatuses(
             readedIds.Contains(book.Id),
             toReadIds.Contains(book.Id)
            );
        }
    }
}