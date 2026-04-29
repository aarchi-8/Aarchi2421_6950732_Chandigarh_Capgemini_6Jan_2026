using BookStore.Application.DTOs;
using FluentValidation;
namespace BookStore.Application.Validators;
public class ReviewCreateValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateValidator() { RuleFor(x => x.BookId).GreaterThan(0); RuleFor(x => x.Rating).InclusiveBetween(1, 5); RuleFor(x => x.Comment).MaximumLength(500); }
}