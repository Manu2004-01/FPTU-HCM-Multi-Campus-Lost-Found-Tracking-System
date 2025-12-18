using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace WebAPI.Models
{
    public class MappingCampus : Profile
    {
        public MappingCampus()
        {
            CreateMap<Campus, CampusDTO>();

            CreateMap<CreateCampusDTO, Campus>()
                .ForMember(d => d.CampusName, o => o.MapFrom(s => s.CampusName.Trim()))
                .ForMember(d => d.CreateAt, o => o.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateCampusDTO, Campus>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
