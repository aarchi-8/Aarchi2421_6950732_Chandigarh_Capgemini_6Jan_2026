using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;
namespace BookStore.Application.MappingProfiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>().ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name)).ForMember(d => d.AuthorName, o => o.MapFrom(s => s.Author.Name)).ForMember(d => d.PublisherName, o => o.MapFrom(s => s.Publisher.Name));
        CreateMap<BookCreateDto, Book>(); CreateMap<BookUpdateDto, Book>().ForAllMembers(o => o.Condition((s, d, v) => v != null));
        CreateMap<Order, OrderResponseDto>().ForMember(d => d.Items, o => o.MapFrom(s => s.OrderItems)).ForMember(d => d.CustomerName, o => o.MapFrom(s => s.User != null ? s.User.FullName : ""));
        CreateMap<OrderItem, OrderItemDto>().ForMember(d => d.BookTitle, o => o.MapFrom(s => s.Book.Title));
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Review, ReviewDto>().ForMember(d => d.BookTitle, o => o.MapFrom(s => s.Book.Title)).ForMember(d => d.UserName, o => o.MapFrom(s => s.User.FullName));
    }
}