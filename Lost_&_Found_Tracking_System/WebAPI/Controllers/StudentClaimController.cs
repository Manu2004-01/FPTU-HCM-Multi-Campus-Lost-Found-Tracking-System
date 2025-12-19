using AutoMapper;
using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Errors;

namespace WebAPI.Controllers
{
    [Route("Claim")]
    [ApiController]
    public class StudentClaimController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentClaimController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("claim")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status201Created)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateClaim([FromForm] CreateClaimDTO claimDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                if(claimDTO == null)
                    return BadRequest(new BaseCommentResponse(400, "Thiếu thông tin claim"));

                // Validate ItemId
                if (claimDTO.ItemId <= 0)
                    return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));

                // Set StudentId từ token để đảm bảo security
                claimDTO.StudentId = userId;

                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));

                var result = await _unitOfWork.ClaimRepository.CreateClaimAsync(claimDTO);

                if (!result)
                {
                    // Kiểm tra lại để xác định lỗi chính xác
                    // Có 2 trường hợp: ItemId không tồn tại hoặc đã claim rồi
                    // Kiểm tra ItemId trước
                    var item = await _unitOfWork.ItemRepository.GetByIdAsync(claimDTO.ItemId);
                    if (item == null)
                    {
                        return BadRequest(new BaseCommentResponse(400, $"ItemId {claimDTO.ItemId} không tồn tại"));
                    }
                    
                    // Nếu item tồn tại, có thể là do duplicate claim
                    return BadRequest(new BaseCommentResponse(400, $"Bạn đã claim item này (ItemId: {claimDTO.ItemId}) rồi. Không thể tạo claim trùng lặp."));
                }

                return Ok(claimDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseCommentResponse(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu"));
            }
        }

        [HttpPut("claim")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClaim([FromQuery] int claimId, [FromForm] UpdateClaimDTO claimDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                if (claimDTO == null)
                    return BadRequest(new BaseCommentResponse(400, "Cần cập nhật dữ liệu"));

                // Kiểm tra xem các field có thực sự được gửi hay không
                if (!Request.Form.ContainsKey("Description") || 
                    (claimDTO.Description != null && claimDTO.Description.Trim().ToLower() == "string"))
                {
                    claimDTO.Description = null;
                }
                if (!Request.Form.ContainsKey("StatusId"))
                {
                    claimDTO.StatusId = null;
                }

                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));

                var result = await _unitOfWork.ClaimRepository.UpdateClaimAsync(claimId, userId, claimDTO);

                if (!result)
                    return NotFound(new BaseCommentResponse(404, "Claim không tồn tại hoặc không thuộc về bạn"));

                return Ok(claimDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseCommentResponse(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu"));
            }
        }

        [HttpDelete("claim")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClaim([FromQuery] int claimId)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                if (claimId <= 0)
                    return BadRequest(new BaseCommentResponse(400, "ClaimId không hợp lệ"));

                var result = await _unitOfWork.ClaimRepository.DeleteClaimAsync(claimId, userId);

                if (!result)
                    return NotFound(new BaseCommentResponse(404, "Claim không tồn tại hoặc không thuộc về bạn"));

                return Ok(new BaseCommentResponse(200, "Xóa claim thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseCommentResponse(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu"));
            }
        }

        [HttpGet("claim")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllClaims()
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                var claims = await _unitOfWork.ClaimRepository.GetAllClaimsAsync(userId);

                return Ok(claims);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseCommentResponse(500, "Đã xảy ra lỗi trong quá trình xử lý yêu cầu"));
            }
        }
    }
}
