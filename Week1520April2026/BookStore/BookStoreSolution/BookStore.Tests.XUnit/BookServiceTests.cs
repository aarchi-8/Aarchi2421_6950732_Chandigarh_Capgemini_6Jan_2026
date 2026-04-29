using Xunit;
using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.MappingProfiles;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using BookStore.Shared;
using Moq;
namespace BookStore.Tests.XUnit;
public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly IMapper _mapper;
    private readonly BookService _service;
    public BookServiceTests() { _bookRepoMock = new Mock<IBookRepository>(); var c = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()); _mapper = c.CreateMapper(); _service = new BookService(_bookRepoMock.Object, _mapper); }

    [Fact] public async Task GetBookById_Existing_ReturnsDto()
    { var b = new Book { BookId = 1, Title = "Test", ISBN = "123", Price = 299, Stock = 10, Category = new() { Name = "F" }, Author = new() { Name = "A" }, Publisher = new() { Name = "P" } }; _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(1)).ReturnsAsync(b); var r = await _service.GetBookByIdAsync(1); Assert.NotNull(r); Assert.Equal("Test", r.Title); }

    [Fact] public async Task GetBookById_NonExisting_ReturnsNull()
    { _bookRepoMock.Setup(r => r.GetBookWithDetailsAsync(999)).ReturnsAsync((Book?)null); Assert.Null(await _service.GetBookByIdAsync(999)); }

    [Fact] public async Task SoftDelete_Existing_SetsFlag()
    { var b = new Book { BookId = 1, IsDeleted = false }; _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(b); _bookRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); Assert.True(await _service.SoftDeleteBookAsync(1)); Assert.True(b.IsDeleted); }

    [Fact] public async Task SoftDelete_NonExisting_ReturnsFalse()
    { _bookRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null); Assert.False(await _service.SoftDeleteBookAsync(999)); }

    [Fact] public async Task GetBooks_ReturnsPaged()
    { var p = new PaginatedResult<Book> { Items = new List<Book> { new() { BookId = 1, Title = "A", Category = new() { Name = "C" }, Author = new() { Name = "A" }, Publisher = new() { Name = "P" } } }, TotalCount = 1, PageNumber = 1, PageSize = 10 }; _bookRepoMock.Setup(r => r.GetPagedBooksAsync(1, 10, null, null)).ReturnsAsync(p); var r = await _service.GetBooksAsync(1, 10, null, null); Assert.Single(r.Items); }
}