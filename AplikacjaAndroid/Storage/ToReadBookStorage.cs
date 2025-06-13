using System.Collections.ObjectModel;

namespace AplikacjaAndroid
{
    public class ToReadBookStorage(ReadedBookStorage readedBookStorage)
    {
        public ObservableCollection<Book> Books { get; } = new();

        public int Count => Books.Count;

        public event EventHandler? CountChanged;

        // Publiczna metoda do wywołania powiadomienia (np. z innego miejsca)
        public void RaiseCountChanged() => CountChanged?.Invoke(this, EventArgs.Empty);

        public void Add(Book book)
        {
            if (!Books.Any(b => b.Id == book.Id))
            {
                Books.Add(book);
                RaiseCountChanged(); // <- tutaj!
            }
        }

        public void Remove(Book book)
        {
            var existing = Books.FirstOrDefault(b => b.Id == book.Id);
            if (existing != null)
            {
                Books.Remove(existing);
                RaiseCountChanged(); // <- tutaj!
            }
        }

        public bool IfBookExists(Book book) =>
            Books.Any(b => b.Id == book.Id);
    }
}
