using Xunit;
using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces;
using BookStore.Application.MappingProfiles;
using BookStore.Application.Services;
using BookStore.Domain.Entities;
using Moq;
namespace BookStore.Tests.XUnit;
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepoMock; private readonly Mock<IBookRepository> _bookRepoMock; private readonly Mock<IEmailService> _emailMock; private readonly IMapper _mapper; private readonly OrderService _service;
    public OrderServiceTests() { _orderRepoMock = new Mock<IOrderRepository>(); _bookRepoMock = new Mock<IBookRepository>(); _emailMock = new Mock<IEmailService>(); var c = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()); _mapper = c.CreateMapper(); _service = new OrderService(_orderRepoMock.Object, _bookRepoMock.Object, _emailMock.Object, _mapper); }

    [Fact] public async Task PlaceOrder_ReducesStock()
    { var b = new Book { BookId = 1, Title = "T", Price = 100, Stock = 10 }; _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(b); _orderRepoMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask); _orderRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1); _orderRepoMock.Setup(r => r.GetOrderWithItemsAsync(It.IsAny<int>())).ReturnsAsync(new Order { OrderId = 1, TotalAmount = 200, Status = "Pending", User = new User { FullName = "T" }, OrderItems = new List<OrderItem> { new() { BookId = 1, Qty = 2, Price = 100, Book = b } } }); var r = await _service.PlaceOrderAsync(1, new OrderCreateDto { Items = new() { new() { BookId = 1, Qty = 2 } } }); Assert.Equal(200, r.TotalAmount); Assert.Equal(8, b.Stock); }

    [Fact] public async Task PlaceOrder_InsufficientStock_Throws()
    { _bookRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Book { BookId = 1, Title = "X", Price = 50, Stock = 1 }); await Assert.ThrowsAsync<InvalidOperationException>(() => _service.PlaceOrderAsync(1, new OrderCreateDto { Items = new() { new() { BookId = 1, Qty = 5 } } })); }

    [Fact] public async Task PlaceOrder_BookNotFound_Throws()
    { _bookRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Book?)null); await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.PlaceOrderAsync(1, new OrderCreateDto { Items = new() { new() { BookId = 999, Qty = 1 } } })); }
}