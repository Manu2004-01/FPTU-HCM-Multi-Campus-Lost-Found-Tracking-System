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
        }
    }
}

