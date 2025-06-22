using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace AplikacjaAndroid
{
    public partial class Book : ObservableObject
    {
        /// <summary>
        /// Constructor to initialize a new instance of the Book class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="author"></param>
        /// <param name="genre"></param>
        /// <param name="publishedDate"></param>
        /// <param name="price"></param>
        /// <param name="stock"></param>
        /// <param name="image"></param>
        /// <param name="description"></param>
        public Book(int id, string title, string author, string genre, DateTime publishedDate, double price, int stock, byte[] image, string? description)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            PublishedDate = publishedDate;
            Price = price;
            Stock = stock;
            Image = image;
            Description = description;
        }

        /// <summary>
        /// Unique identifier for the book
        /// </summary>        
        public int Id { get; set; }
        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Author of the book
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Genre of the book
        /// </summary>
        public string Genre { get; set; }
        /// <summary>
        /// Date when the book was published
        /// </summary>
        public DateTime PublishedDate { get; set; }
        /// <summary>
        /// Price of the book
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Stock quantity of the book
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// Image of the book
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        /// Description of the book
        /// </summary>
        public string? Description { get; set; }

        [JsonIgnore]
        private ImageSource? _cachedImageSource;

        [JsonIgnore]
        public ImageSource? ImageSource
        {
            get
            {
                if (_cachedImageSource == null && Image != null && Image.Length > 0)
                    _cachedImageSource = ImageSource.FromStream(() => new MemoryStream(Image));
                return _cachedImageSource;
            }
        }

        [JsonIgnore]
        [ObservableProperty]
        bool _isReaded;

        [JsonIgnore]
        [ObservableProperty]
        bool _isToRead;

        public void UpdateStatuses(bool isReaded, bool isToRead)
        {
            IsReaded = isReaded;
            IsToRead = isToRead;
        }
    }
}
