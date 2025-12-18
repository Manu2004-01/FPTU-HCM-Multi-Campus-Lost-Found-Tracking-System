using AutoMapper;
using Core.DTOs;
using Core.Entities;
using WebAPI.Helpers;

namespace WebAPI.Models
{
    public class MappingClaim : Profile
    {
        public MappingClaim()
        {
            CreateMap<CreateClaimDTO, Claim>()
                .ForMember(d => d.StudentId, opt => opt.MapFrom(s => s.StudentId))
                .ForMember(d => d.ItemId, opt => opt.MapFrom(s => s.ItemId))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.StatusId, opt => opt.MapFrom(s => s.StatusId))
                .ForMember(dest => dest.EvidenceImageUrl, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Claim, StudentClaimDTO>()
                .ForMember(d => d.ClaimId, opt => opt.MapFrom(s => s.ClaimId))
                .ForMember(d => d.ItemDescription, opt => opt.MapFrom(s => s.Item != null ? s.Item.Description : null))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.EvidenceImageUrl, static opt => opt.MapFrom<ClaimImageUrlResolver>())
                .ForMember(d => d.ClaimStatus, opt => opt.MapFrom(s => s.StatusId))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt))
                .ReverseMap();

            CreateMap<Claim, StaffClaimQueueDTO>()
                .ForMember(d => d.ItemTitle, o => o.MapFrom(s => s.Item.Title))
                .ForMember(d => d.ItemDescription, o => o.MapFrom(s => s.Item.Description))
                .ForMember(d => d.ItemImageUrl, o => o.MapFrom(s => s.Item.ImageUrl))
                .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.Fullname))
                .ForMember(d => d.StudentEmail, o => o.MapFrom(s => s.Student.Email))
                .ForMember(d => d.ClaimDescription, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.StatusName, o => o.MapFrom(s => s.Status.StatusName));
        }
    }
}

