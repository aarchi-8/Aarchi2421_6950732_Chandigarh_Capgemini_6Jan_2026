using AutoMapper;
using EventBooking.API.DTOs;
using EventBooking.API.Entities;

namespace EventBooking.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Event mappings
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventDto, Event>();

        // Booking mappings
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.EventTitle,    opt => opt.MapFrom(src => src.Event != null ? src.Event.Title    : string.Empty))
            .ForMember(dest => dest.EventLocation, opt => opt.MapFrom(src => src.Event != null ? src.Event.Location : string.Empty))
            .ForMember(dest => dest.EventDate,     opt => opt.MapFrom(src => src.Event != null ? src.Event.Date     : default));

        CreateMap<CreateBookingDto, Booking>();
    }
}
