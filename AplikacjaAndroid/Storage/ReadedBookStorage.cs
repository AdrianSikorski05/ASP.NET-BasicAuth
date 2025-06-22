using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    public class ReadedBookStorage(IBooksService booksService, UserStorage userStorage) 
    {
        public ObservableCollection<Book> Books { get; set; } = new();

        public int Count => Books.Count;

        public event EventHandler? CountChanged;

        // Publiczna metoda do wywołania powiadomienia (np. z innego miejsca)
        public void RaiseCountChanged() => CountChanged?.Invoke(this, EventArgs.Empty);
        public async Task Add(Book book)
        {
            if (!Books.Any(b => b.Id == book.Id))
            {
                Books.Add(book);
                await booksService.UpdateBookActivityStatus(book, BookStatus.AddedToReaded);
                RaiseCountChanged();
            }
        }

        public async Task Remove(Book book)
        {
            var existing = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            { 
                Books.Remove(existing);
                await booksService.UpdateBookActivityStatus(book, BookStatus.DeletedFromReaded);
                RaiseCountChanged();
            }
        }

        public bool IfBookExists(Book book) =>
            Books.Any(b => b.Id == book.Id);


        public async Task LoadData()
        {

            if (userStorage.User == null)
            {
                Console.WriteLine("User is null, cannot load book data.");
                return;
            }

            var data = new DataStatusBookWithUserIdDto
            {
                UserId = userStorage.User.Id,
                StatusBook = "readed"
            };

            var response = await booksService.GetBookWithActivityStatusByUser(data);

            if (response != null && response.StatusCode == 200 && response.Data != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Books = new ObservableCollection<Book>(response.Data);
                    RaiseCountChanged();
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Books = new ObservableCollection<Book>();
                    RaiseCountChanged();
                });
            }
        }
    }
}
