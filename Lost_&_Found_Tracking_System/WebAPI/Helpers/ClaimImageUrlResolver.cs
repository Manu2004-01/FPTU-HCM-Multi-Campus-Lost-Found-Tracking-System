using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Helpers
{
    public class ClaimImageUrlResolver : IValueResolver<Claim, StudentClaimDTO, string>
    {
        private readonly IConfiguration _configuration;

        public ClaimImageUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Claim source, StudentClaimDTO destination, string destMember, ResolutionContext context)
        {
            if (source == null || string.IsNullOrEmpty(source.EvidenceImageUrl))
                return null;

            var apiUrl = _configuration["API_url"] ?? string.Empty;
            return apiUrl + source.EvidenceImageUrl;
        }
    }
}
