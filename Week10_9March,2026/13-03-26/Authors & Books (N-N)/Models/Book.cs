using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Authors___Books.Models

{
    [Table("Books")]
    public class Book
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }

        public List<AuthorBook>? AuthorBooks { get; set; }
    }
}
