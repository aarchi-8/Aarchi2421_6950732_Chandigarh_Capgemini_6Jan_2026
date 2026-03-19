using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Range(1000, 2100)]
        public int PublishedYear { get; set; }

        public string Genre { get; set; }
    }
}