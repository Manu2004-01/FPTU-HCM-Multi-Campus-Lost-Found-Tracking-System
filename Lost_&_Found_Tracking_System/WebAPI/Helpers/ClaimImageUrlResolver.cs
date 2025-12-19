using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Helpers
{
    public class ClaimImageUrlResolver : IValueResolver<Claim, StudentClaimDTO, string>
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimImageUrlResolver(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(Claim source, StudentClaimDTO destination, string destMember, ResolutionContext context)
        {
            if (source == null || string.IsNullOrEmpty(source.EvidenceImageUrl))
                return null;

            // Auto-detect API URL from current request, fallback to config
            var apiUrl = GetApiUrl();
            // source.EvidenceImageUrl already starts with "/", so just combine
            return apiUrl + source.EvidenceImageUrl;
        }

        private string GetApiUrl()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var request = httpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
                // Remove trailing slash to avoid double slashes when combining with imageUrl
                return baseUrl.TrimEnd('/');
            }

            // Fallback to configuration
            var configUrl = _configuration["API_url"] ?? string.Empty;
            return configUrl.TrimEnd('/');
        }
    }
}
