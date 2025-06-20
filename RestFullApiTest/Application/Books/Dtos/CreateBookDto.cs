﻿using System.ComponentModel.DataAnnotations;

namespace RestFullApiTest
{
    public class CreateBookDto
    {
        public CreateBookDto(string title, string author, string genre, double price, int stock, byte[] image, string? description)
        {
            Title = title;
            Author = author;
            Genre = genre;
            Price = price;
            Stock = stock;
            Image = image;
            Description = description;
        }

        /// <summary>
        /// The title of the book.
        /// </summary>
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = null!;
        /// <summary>
        /// The author of the book.
        /// </summary>
        [Required(ErrorMessage = "Author is required")]
        public string Author { get; set; } = null!;
        /// <summary>
        /// The genre of the book.
        /// </summary>
        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; } = null!;
        /// <summary>
        /// The price of the book.
        /// </summary>
        [Required(ErrorMessage = "Price is required")]
        public double Price { get; set; }
        /// <summary>
        /// The stock of the book.
        /// </summary>
        [Required(ErrorMessage = "Stock is required")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public byte[] Image { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }
    }
}
