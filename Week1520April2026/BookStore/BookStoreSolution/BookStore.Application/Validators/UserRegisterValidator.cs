using BookStore.Application.DTOs;
using FluentValidation;
namespace BookStore.Application.Validators;
public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).Matches(@"[A-Z]").Matches(@"[a-z]").Matches(@"[0-9]").Matches(@"[^a-zA-Z0-9]");
        RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d{10}$");
    }
}