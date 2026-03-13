using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Authors___Books.Models
{
    [Table("Authors")]
    public class Author
    {
        public int AuthorId { get; set; }
        [Required]
        public string Name { get; set; }    
        public List<AuthorBook>? AuthorBooks { get; set; }
    }
}
