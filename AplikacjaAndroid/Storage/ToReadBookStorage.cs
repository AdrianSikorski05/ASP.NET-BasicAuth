using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AplikacjaAndroid
{
    public class ToReadBookStorage(IBooksService booksService, UserStorage userStorage)
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
                await booksService.UpdateBookActivityStatus(book, BookStatus.AddedToToRead);
                RaiseCountChanged(); // <- tutaj!
            }
        }

        public async Task Remove(Book book)
        {
            var existing = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                Books.Remove(existing);
                await booksService.UpdateBookActivityStatus(book, BookStatus.DeletedFromToRead);
                RaiseCountChanged(); // <- tutaj!
            }
        }

        public bool IfBookExists(Book book) =>
            Books.Any(b => b.Id == book.Id);

        public async Task LoadData()
        {
            var data = new DataStatusBookWithUserIdDto
            {
                UserId = userStorage.User.Id,
                StatusBook = "toRead"
            };

            var response = await booksService.GetBookWithActivityStatusByUser(data);

            if (response != null && response.StatusCode == 200 && response.Data != null)
            {
                Books = new ObservableCollection<Book>(response.Data);
            }
            else
            {
                Books = new ObservableCollection<Book>();
                Console.WriteLine($"Błąd: {response?.Message ?? "Brak odpowiedzi z serwera"}");
            }
            RaiseCountChanged();
        }
    }
}
