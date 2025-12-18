using Core.DTOs;
using Core.Interfaces;
using Core.Sharing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Errors;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("staff/items")]
    [Authorize(Roles = "staff")]
    public class StaffInventoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffInventoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems(
            [FromQuery] int? itemTypeId = null,
            [FromQuery] int? statusId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? campusId = null,
            [FromQuery] string? sorting = null)
        {
            var items = await _unitOfWork.ItemRepository.GetItemDashboardAsync(new EntityParam
            {
                ItemTypeId = itemTypeId,
                StatusId = statusId,
                CategoryId = categoryId,
                CampusId = campusId,
                Sorting = sorting
            });

            return Ok(items);
        }

        [HttpPut("{id:int}/verify")]
        public async Task<IActionResult> VerifyItem([FromRoute] int id, [FromBody] StaffVerifyItemDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (id <= 0) return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));

            var ok = await _unitOfWork.ItemRepository.VerifyItemAsync(id, staffId.Value, dto?.Notes);
            if (!ok) return NotFound(new BaseCommentResponse(404, "Không tìm thấy item hoặc không thể verify"));

            return Ok(new BaseCommentResponse(200, "Verify item thành công"));
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] StaffUpdateItemStatusDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (id <= 0) return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));
            if (dto == null || dto.NewStatusId <= 0)
                return BadRequest(new BaseCommentResponse(400, "NewStatusId không hợp lệ"));

            var ok = await _unitOfWork.ItemRepository.UpdateItemStatusAsync(id, staffId.Value, dto.NewStatusId, dto.Notes);
            if (!ok) return BadRequest(new BaseCommentResponse(400, "Không thể cập nhật status (Item/Status không tồn tại)"));

            return Ok(new BaseCommentResponse(200, "Cập nhật trạng thái thành công"));
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
