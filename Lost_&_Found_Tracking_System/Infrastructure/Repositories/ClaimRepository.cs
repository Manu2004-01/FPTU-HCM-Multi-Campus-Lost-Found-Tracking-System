using AutoMapper;
using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ClaimRepository : GenericRepository<Claim>, IClaimRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;

        public ClaimRepository(ApplicationDBContext context, IFileProvider fileProvider, IMapper mapper) : base(context)
        {
            _context = context;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        public async Task<bool> CreateClaimAsync(CreateClaimDTO claimDTO)
        {
            // Validate ItemId tồn tại
            var itemExists = await _context.Items.AnyAsync(i => i.ItemId == claimDTO.ItemId);
            if (!itemExists)
            {
                return false; // ItemId không tồn tại
            }

            // Kiểm tra xem student đã claim item này chưa (tránh duplicate claim từ cùng 1 student)
            var existingClaim = await _context.Claims
                .AnyAsync(c => c.ItemId == claimDTO.ItemId && c.StudentId == claimDTO.StudentId);
            
            if (existingClaim)
            {
                return false; // Student đã claim item này rồi
            }

            var claim = _mapper.Map<Claim>(claimDTO);
            
            // Đảm bảo ItemId và StudentId được set đúng (không bị thay đổi bởi mapping)
            claim.ItemId = claimDTO.ItemId;
            claim.StudentId = claimDTO.StudentId;

            if (claimDTO.EvidenceImage != null)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(claimDTO.EvidenceImage.FileName)}";

                var relativePath = Path.Combine("images", "claims", fileName);

                var fileInfo = _fileProvider.GetFileInfo(relativePath);

                var physicalPath = fileInfo.PhysicalPath;

                var directory = Path.GetDirectoryName(physicalPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await claimDTO.EvidenceImage.CopyToAsync(stream);
                }

                claim.EvidenceImageUrl = $"/images/claims/{fileName}";
            }
            else
            {
                claim.EvidenceImageUrl = null;
            }

            var now = DateTime.UtcNow;
            claim.CreatedAt = now;
            claim.UpdatedAt = now;

            await _context.Claims.AddAsync(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateClaimAsync(int claimId, Guid userId, UpdateClaimDTO claimDTO)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimId == claimId && c.StudentId == userId);
            
            if (claim == null)
                return false; // Claim không tồn tại hoặc không thuộc về user này

            // Chỉ update các trường có giá trị
            if (!string.IsNullOrWhiteSpace(claimDTO.Description))
            {
                claim.Description = claimDTO.Description.Trim();
            }

            // Update StatusId nếu có giá trị
            if (claimDTO.StatusId.HasValue && claimDTO.StatusId.Value > 0)
            {
                claim.StatusId = claimDTO.StatusId.Value;
            }

            // Xử lý file upload nếu có
            if (claimDTO.EvidenceImage != null)
            {
                // Xóa file cũ nếu có
                if (!string.IsNullOrEmpty(claim.EvidenceImageUrl))
                {
                    try
                    {
                        var oldFileInfo = _fileProvider.GetFileInfo(claim.EvidenceImageUrl);
                        if (oldFileInfo.Exists)
                        {
                            File.Delete(oldFileInfo.PhysicalPath);
                        }
                    }
                    catch
                    {
                        // Ignore nếu không xóa được file cũ
                    }
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(claimDTO.EvidenceImage.FileName)}";
                var relativePath = Path.Combine("images", "claims", fileName);
                var fileInfo = _fileProvider.GetFileInfo(relativePath);
                var physicalPath = fileInfo.PhysicalPath;
                var directory = Path.GetDirectoryName(physicalPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    await claimDTO.EvidenceImage.CopyToAsync(stream);
                }

                claim.EvidenceImageUrl = $"/images/claims/{fileName}";
            }

            claim.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteClaimAsync(int claimId, Guid userId)
        {
            var claim = await _context.Claims
                .FirstOrDefaultAsync(c => c.ClaimId == claimId && c.StudentId == userId);
            
            if (claim == null)
                return false; // Claim không tồn tại hoặc không thuộc về user này

            // Xóa file ảnh nếu có
            if (!string.IsNullOrEmpty(claim.EvidenceImageUrl))
            {
                try
                {
                    var fileInfo = _fileProvider.GetFileInfo(claim.EvidenceImageUrl);
                    if (fileInfo.Exists)
                    {
                        File.Delete(fileInfo.PhysicalPath);
                    }
                }
                catch
                {
                    // Ignore nếu không xóa được file
                }
            }

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StudentClaimDTO>> GetAllClaimsAsync(Guid userId)
        {
            var claims = await _context.Claims
                .AsNoTracking()
                .Include(c => c.Item)
                .Include(c => c.Status)
                .Where(c => c.StudentId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<StudentClaimDTO>>(claims);
        }

        public async Task<IEnumerable<StaffClaimQueueDTO>> GetClaimQueueAsync(int? statusId = null)
        {
            var query = _context.Claims
                .AsNoTracking()
                .Include(c => c.Item)
                .Include(c => c.Status)
                .Include(c => c.Student)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(c => c.StatusId == statusId.Value);

            var list = await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<StaffClaimQueueDTO>>(list);
        }

        public async Task<bool> VerifyClaimAsync(int claimId, Guid staffId, int newStatusId, string? notes)
        {
            var statusExists = await _context.ClaimStatuses.AnyAsync(s => s.Id == newStatusId);
            if (!statusExists) return false;

            var claim = await _context.Claims.FirstOrDefaultAsync(c => c.ClaimId == claimId);
            if (claim == null) return false;

            claim.StatusId = newStatusId;
            claim.UpdatedAt = DateTime.UtcNow;

            await _context.VerificationLogs.AddAsync(new VerificationLog
            {
                ItemId = claim.ItemId,
                ClaimId = claim.ClaimId,
                VerifiedByUserId = staffId,
                VerificationType = "claim_verify",
                Notes = notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResolveConflictAsync(int itemId, int winnerClaimId, Guid staffId, string? notes)
        {
            var winner = await _context.Claims.FirstOrDefaultAsync(c => c.ClaimId == winnerClaimId && c.ItemId == itemId);
            if (winner == null) return false;

            // Set winner to be approved
            winner.StatusId = 2; // approved
            winner.UpdatedAt = DateTime.UtcNow;

            // Set other claims to be rejected
            var others = await _context.Claims
                .Where(c => c.ItemId == itemId && c.ClaimId != winnerClaimId)
                .ToListAsync();

            foreach (var c in others)
            {
                c.StatusId = 3; // rejected
                c.UpdatedAt = DateTime.UtcNow;
            }

            await _context.VerificationLogs.AddAsync(new VerificationLog
            {
                ItemId = itemId,
                ClaimId = winnerClaimId,
                VerifiedByUserId = staffId,
                VerificationType = "claim_conflict_resolve",
                Notes = notes ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
