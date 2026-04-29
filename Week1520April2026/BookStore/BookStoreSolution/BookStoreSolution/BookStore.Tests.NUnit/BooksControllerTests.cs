using BookStore.API.Controllers;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
namespace BookStore.Tests.NUnit;
[TestFixture] public class BooksControllerTests
{
    private Mock<IBookService> _mock = null!; private BooksController _ctrl = null!;
    [SetUp] public void Setup() { _mock = new Mock<IBookService>(); _ctrl = new BooksController(_mock.Object); }
    [Test] public async Task GetBooks_ReturnsOk() { _mock.Setup(s => s.GetBooksAsync(1, 10, null, null)).ReturnsAsync(new PaginatedResult<BookDto> { Items = new() { new() { BookId = 1, Title = "B", AuthorName = "A", CategoryName = "C", PublisherName = "P" } }, TotalCount = 1, PageNumber = 1, PageSize = 10 }); var r = await _ctrl.GetBooks(); Assert.That(r.Result, Is.TypeOf<OkObjectResult>()); }
    [Test] public async Task GetBook_NotFound() { _mock.Setup(s => s.GetBookByIdAsync(999)).ReturnsAsync((BookDto?)null); var r = await _ctrl.GetBook(999); Assert.That(r.Result, Is.TypeOf<NotFoundObjectResult>()); }
    [Test] public async Task Delete_Existing_ReturnsOk() { _mock.Setup(s => s.SoftDeleteBookAsync(1)).ReturnsAsync(true); var r = await _ctrl.DeleteBook(1); Assert.That(r.Result, Is.TypeOf<OkObjectResult>()); }
}