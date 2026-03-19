using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace StudentManagement.Models
{
    [Table("NewStudent")]
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(18, 60)]
        public int Age { get; set; }

        public string Course { get; set; }
    }
}
