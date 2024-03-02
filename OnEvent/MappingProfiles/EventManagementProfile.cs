using AutoMapper;
using DataAccess.UnitOfWork.Classes;
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
            CreateMap(typeof(PagedList<>), typeof(ViewPagedList<>));
        }
    }
}
