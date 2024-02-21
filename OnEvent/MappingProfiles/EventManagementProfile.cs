using AutoMapper;
using Models.DataTransferObjects;
using Models.Models;

namespace OnEvent.MappingProfiles
{
    public class EventManagementProfile : Profile
    {
        public EventManagementProfile()
        {
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Guest, GuestDto>().ReverseMap();
        }
    }
}
