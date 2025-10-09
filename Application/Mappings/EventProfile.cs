using AutoMapper;
using SNI_Events.Application.Dtos.Event;
using SNI_Events.Domain.Entities;

namespace SNI_Events.Application.Mappings
{
    public class EventProfile : Profile // Extend AutoMapper's Profile class
    {
        public EventProfile() // Provide a constructor with a body
        {
            CreateMap<Event, EventDto>();
            CreateMap<EventCreateRequestDto, Event>();
            CreateMap<EventUpdateRequestDto, Event>();
        }
    }
}
