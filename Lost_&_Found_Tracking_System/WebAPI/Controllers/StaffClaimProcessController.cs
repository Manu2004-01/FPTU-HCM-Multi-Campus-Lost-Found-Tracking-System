using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Errors;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("staff/claims")]
    [Authorize(Roles = "staff")]
    public class StaffClaimProcessController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffClaimProcessController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetQueue([FromQuery] int? statusId = null)
        {
            var list = await _unitOfWork.ClaimRepository.GetClaimQueueAsync(statusId);
            return Ok(list);
        }

        [HttpPut("{claimId:int}/verify")]
        public async Task<IActionResult> VerifyClaim([FromRoute] int claimId, [FromBody] StaffVerifyClaimDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (claimId <= 0) return BadRequest(new BaseCommentResponse(400, "ClaimId không hợp lệ"));
            if (dto == null || dto.NewStatusId <= 0)
                return BadRequest(new BaseCommentResponse(400, "NewStatusId không hợp lệ"));

            var ok = await _unitOfWork.ClaimRepository.VerifyClaimAsync(claimId, staffId.Value, dto.NewStatusId, dto.Notes);
            if (!ok) return BadRequest(new BaseCommentResponse(400, "Không thể duyệt claim (Claim/Status không tồn tại)"));

            return Ok(new BaseCommentResponse(200, "Duyệt claim thành công"));
        }

        // Resolve conflict for an item: choose winner claimId
        [HttpPut("{itemId:int}/resolve-conflict")]
        public async Task<IActionResult> ResolveConflict([FromRoute] int itemId, [FromBody] StaffResolveConflictDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (itemId <= 0) return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));
            if (dto == null || dto.WinnerClaimId <= 0)
                return BadRequest(new BaseCommentResponse(400, "WinnerClaimId không hợp lệ"));

            var ok = await _unitOfWork.ClaimRepository.ResolveConflictAsync(itemId, dto.WinnerClaimId, staffId.Value, dto.Notes);
            if (!ok) return BadRequest(new BaseCommentResponse(400, "Không thể xử lý conflict (Item/Claim không hợp lệ)"));

            return Ok(new BaseCommentResponse(200, "Xử lý conflict thành công"));
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
