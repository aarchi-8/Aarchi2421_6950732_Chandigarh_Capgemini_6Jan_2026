using LibraryApp.Models;

namespace LibraryApp.ViewModels
{
    public class BookViewModel
    {
        public Book Book { get; set; }

        public bool IsAvailable { get; set; }

        // Practice extension
        public string BorrowerName { get; set; }
    }
}