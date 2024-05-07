using AutoMapper;
using DataAccess.UnitOfWork.Classes;
using Models.DataTransferObjects;
using Models.Models;

namespace OnEvent.MappingProfiles
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Guest, GuestDto>().ReverseMap();
            CreateMap<Invitation, InvitationDto>().ReverseMap();
            CreateMap(typeof(PagedList<>), typeof(ViewPagedList<>));
        }
    }
}
