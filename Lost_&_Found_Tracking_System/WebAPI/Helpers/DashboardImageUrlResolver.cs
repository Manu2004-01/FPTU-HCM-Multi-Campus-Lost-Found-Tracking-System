using AutoMapper;
using Core.DTOs;
using Core.Entities;

namespace WebAPI.Helpers
{
    public class DashboardImageUrlResolver : IValueResolver<Item, ItemDTO, string>
    {
        private readonly IConfiguration _configuration;

        public DashboardImageUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Item source, ItemDTO destination, string destMember, ResolutionContext context)
        {
            if (source == null || string.IsNullOrEmpty(source.ImageUrl))
                return null;

            var apiUrl = _configuration["API_url"] ?? string.Empty;
            return apiUrl + source.ImageUrl;
        }
    }
}
