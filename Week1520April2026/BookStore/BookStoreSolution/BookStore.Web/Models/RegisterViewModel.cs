using System.ComponentModel.DataAnnotations;
namespace BookStore.Web.Models;
public class RegisterViewModel
{
    [Required, MaxLength(100)] public string FullName { get; set; } = "";
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, MinLength(8), RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$", ErrorMessage = "Password needs uppercase, lowercase, number, and special character.")] public string Password { get; set; } = "";
    [Required, Phone] public string Phone { get; set; } = "";
}