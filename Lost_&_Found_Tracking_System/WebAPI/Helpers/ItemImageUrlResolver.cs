using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Helpers
{
    public class ItemImageUrlResolver : IValueResolver<Item, StudentItemDTO, string>
    {
        private readonly IConfiguration _configuration;

        public ItemImageUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Item source, StudentItemDTO destination, string destMember, ResolutionContext context)
        {
            if (source == null || string.IsNullOrEmpty(source.ImageUrl))
                return null;

            var apiUrl = _configuration["API_url"] ?? string.Empty;
            return apiUrl + source.ImageUrl;
        }
    }
}
