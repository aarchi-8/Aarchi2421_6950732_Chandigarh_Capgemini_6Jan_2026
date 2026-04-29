using System.ComponentModel.DataAnnotations;
namespace BookStore.Web.Models;
public class LoginViewModel { [Required, EmailAddress] public string Email { get; set; } = ""; [Required, MinLength(8)] public string Password { get; set; } = ""; }