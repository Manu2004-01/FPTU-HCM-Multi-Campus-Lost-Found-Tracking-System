using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClaimRepository : IGenericRepository<Claim>
    {
        Task<bool> CreateClaimAsync(CreateClaimDTO claimDTO);
        Task<bool> UpdateClaimAsync(int claimId, Guid userId, UpdateClaimDTO claimDTO);
        Task<bool> DeleteClaimAsync(int claimId, Guid userId);
        Task<IEnumerable<StudentClaimDTO>> GetAllClaimsAsync(Guid userId);

        // Staff
        Task<IEnumerable<StaffClaimQueueDTO>> GetClaimQueueAsync(int? statusId = null);
        Task<bool> VerifyClaimAsync(int claimId, Guid staffId, int newStatusId, string? notes);
        Task<bool> ResolveConflictAsync(int itemId, int winnerClaimId, Guid staffId, string? notes);
    }
}
