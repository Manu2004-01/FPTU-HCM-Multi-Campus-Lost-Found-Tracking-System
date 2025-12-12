using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClaimRepository
    {
        Task<bool> CreateClaimAsync(CreateClaimDTO claimDTO);
        Task<bool> UpdateClaimAsync(int claimId, Guid userId, UpdateClaimDTO claimDTO);
        Task<bool> DeleteClaimAsync(int claimId, Guid userId);
        Task<IEnumerable<StudentClaimDTO>> GetAllClaimsAsync(Guid userId);
    }
}
