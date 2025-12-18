using AutoMapper;
using Core.DTOs;
using Core.Entities;
using WebAPI.Helpers;

namespace WebAPI.Models
{
    public class MappingItem : Profile
    {
        public MappingItem()
        {
            CreateMap<Item, StudentItemDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ItemId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.ImageUrl, static opt => opt.MapFrom<ItemImageUrlResolver>())
                .ForMember(d => d.ItemTypeName, opt => opt.MapFrom(s => s.ItemType != null ? s.ItemType.TypeName : null))
                .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status != null ? s.Status.StatusName : null))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null))
                .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.CampusName : null))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
                .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(s => s.UpdatedAt))
                .ReverseMap();

            CreateMap<CreateItemDTO, Item>()
                .ForMember(d => d.LostByStudentId, opt => opt.MapFrom(s => s.LostByStudentId))
                .ForMember(d => d.FoundByUserId, opt => opt.MapFrom(s => s.FoundByUserId))
                .ForMember(d => d.ItemTypeId, opt => opt.MapFrom(s => s.ItemTypeId))
                .ForMember(d => d.StatusId, opt => opt.MapFrom(s => s.StatusId))
                .ForMember(d => d.CategoryId, opt => opt.MapFrom(s => s.CategoryId))
                .ForMember(d => d.CampusId, opt => opt.MapFrom(s => s.CampusId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdateItemDTO, Item>()
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Item, ItemDTO>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ItemId))
                .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.ImageUrl, static opt => opt.MapFrom<DashboardImageUrlResolver>())
                .ForMember(d => d.StatusName, opt => opt.MapFrom(s => s.Status != null ? s.Status.StatusName : null))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null))
                .ForMember(d => d.CampusName, opt => opt.MapFrom(s => s.Campus != null ? s.Campus.CampusName : null))
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(s => s.CreatedAt))
                .ReverseMap();
        }
    }
}
