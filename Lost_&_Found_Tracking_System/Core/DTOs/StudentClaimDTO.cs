using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class StudentClaimDTO
    {
        public int ClaimId { get; set; }
        public string ItemDescription { get; set; }
        public string Description { get; set; }
        public string EvidenceImageUrl { get; set; }
        public int ClaimStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateClaimDTO
    {
        private int _claimstatusId = 1;

        public Guid StudentId { get; set; }
        public int ItemId { get; set; }
        public string Description { get; set; }
        public IFormFile EvidenceImage { get; set; }
        public int StatusId
        {
            get => _claimstatusId;
            set
            {
                _claimstatusId = value == 0 ? 1 : value;
            }
        }
    }

    public class UpdateClaimDTO
    {
        public string Description { get; set; }
        public IFormFile? EvidenceImage { get; set; }
        public int? StatusId { get; set; }
    }
}
