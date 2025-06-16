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

    private List<Book> _allLoadedBooks = new List<Book>();

    public BooksContext(IBooksService booksService, BookMenuPopup bookMenuPopup, ReadedBookStorage readedBookStorage, ToReadBookStorage toReadBookStorage)
    {
        _booksService = booksService;
        _bookMenuPopup = bookMenuPopup;
        _readedBookStorage = readedBookStorage;
        _toReadBookStorage = toReadBookStorage;

        _ = LoadBooksFromServerAsync();
    }

    [ObservableProperty]
    BookFilter _bookFilter = new();

    [ObservableProperty]
    bool _isLoading;

    [ObservableProperty]
    bool _visibilityButtonNextPage = true;

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
        VisibilityButtonNextPage = false;
        IsLoading = true;
        BookFilter.Page = 1;

        var response = await _booksService.GetAllBooks(BookFilter);

        if (response != null && response.StatusCode == 200 && response.Data?.Items != null)
        {
            _allLoadedBooks = response.Data.Items.ToList();
            Books = new ObservableCollection<Book>(_allLoadedBooks);
            VisibilityButtonNextPage = response.Data.Items.Count() == BookFilter.PageSize;
            SetBookStatuses(Books);
        }
        else
        {
            _allLoadedBooks = new List<Book>();
            Books = new ObservableCollection<Book>();
            VisibilityButtonNextPage = false;
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task GoToDetailsView(Book selectedBook)
    {
        await Shell.Current.GoToAsync(nameof(DetailsBookView), true, new Dictionary<string, object>
        {
            { "Book", selectedBook },
            { "IsVisibleButtons", true },
            { "IsVisibleConfig", false }
        });
    }

    [RelayCommand]
    public async Task LoadNextPage()
    {
        IsLoading = true;
        VisibilityButtonNextPage = false;

        BookFilter.Page++;

        var result = await _booksService.GetAllBooks(BookFilter);

        if (result?.Data?.Items != null && result.Data.Items.Any())
        {
            SetBookStatuses(result.Data.Items);
            foreach (var item in result.Data.Items)
            {
                Books.Add(item);
            }
            _allLoadedBooks.AddRange(result.Data.Items);
            VisibilityButtonNextPage = result.Data.Items.Count() == BookFilter.PageSize;
        }
        else
        {
            VisibilityButtonNextPage = false;
        }
        IsLoading = false;
    }

    [RelayCommand]
    public async Task ShowPopup(Book book)
    {
        _bookMenuPopup.LoadContext(book, false);
        await MopupService.Instance.PushAsync(_bookMenuPopup, true);
        await Task.Delay(1000);
        SetBookStatuses(Books);

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