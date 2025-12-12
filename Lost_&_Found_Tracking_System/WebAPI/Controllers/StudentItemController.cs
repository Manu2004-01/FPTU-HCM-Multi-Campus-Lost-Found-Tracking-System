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
    [Route("Item")]
    [ApiController]
    public class StudentItemController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentItemController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost("item")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status201Created)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateItem([FromForm] CreateItemDTO itemDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                if (itemDTO == null)
                    return BadRequest(new BaseCommentResponse(400, "Thiếu thông tin item"));

                // Validate ItemTypeId (chỉ cho phép 1 hoặc 2)
                if (itemDTO.ItemTypeId != 1 && itemDTO.ItemTypeId != 2)
                {
                    return BadRequest(new BaseCommentResponse(400, "ItemTypeId phải là 1 (Lost) hoặc 2 (Found)"));
                }

                // Validate CampusId (phải > 0)
                if (itemDTO.CampusId <= 0)
                {
                    return BadRequest(new BaseCommentResponse(400, "CampusId phải lớn hơn 0"));
                }

                // Xử lý logic dựa trên ItemTypeId
                if (itemDTO.ItemTypeId == 1) // Lost Item
                {
                    itemDTO.LostByStudentId = userId;
                    itemDTO.FoundByUserId = null;
                }
                else if (itemDTO.ItemTypeId == 2) // Found Item
                {
                    itemDTO.FoundByUserId = userId;
                    itemDTO.LostByStudentId = null;
                }

                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));

                var result = await _unitOfWork.ItemRepository.CreateItemAsync(itemDTO);

                if (!result)
                    return StatusCode(500, new BaseCommentResponse(500, "Tạo item không thành công"));

                return Ok(itemDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ"));
            }
        }

        [HttpPut("item")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem([FromQuery] int itemId, [FromForm] UpdateItemDTO itemDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));
                if (itemDTO == null)
                    return BadRequest(new BaseCommentResponse(400, "Cần cập nhật dữ liệu"));

                // Kiểm tra xem các field có thực sự được gửi hay không
                // Nếu không được gửi hoặc là giá trị mặc định "string", set thành null để repository biết không cần update
                if (!Request.Form.ContainsKey("Title") ||
                    (itemDTO.Title != null && itemDTO.Title.Trim().ToLower() == "string"))
                {
                    itemDTO.Title = null;
                }
                if (!Request.Form.ContainsKey("Description") ||
                    (itemDTO.Description != null && itemDTO.Description.Trim().ToLower() == "string"))
                {
                    itemDTO.Description = null;
                }
                if (!Request.Form.ContainsKey("CategoryId"))
                {
                    itemDTO.CategoryId = null;
                }
                if (!Request.Form.ContainsKey("CampusId"))
                {
                    itemDTO.CampusId = null;
                }

                var updateResult = await _unitOfWork.ItemRepository.UpdateItemAsync(itemId, itemDTO);
                if (!updateResult)
                    return NotFound(new BaseCommentResponse(404, "Item không tồn tại"));
                return Ok(itemDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ"));
            }
        }

        [HttpDelete("item")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem([FromQuery] int itemId)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }
                if (itemId <= 0)
                    return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));
                var deleteResult = await _unitOfWork.ItemRepository.DeleteItemAsync(itemId);
                if (!deleteResult)
                    return NotFound(new BaseCommentResponse(404, "Item không tồn tại hoặc xóa không thành công"));
                return Ok(new BaseCommentResponse(200, "Xóa item thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ"));
            }
        }

        [HttpGet("item_report")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllReport([FromQuery] int? itemTypeId = null, [FromQuery] int? statusId = null, [FromQuery] int? categoryId = null, [FromQuery] int? campusId = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }
                var items = await _unitOfWork.ItemRepository.GetAllItemsAsync(new Core.Sharing.EntityParam
                {
                    ItemTypeId = itemTypeId,
                    StatusId = statusId,
                    CategoryId = categoryId,
                    CampusId = campusId,
                    UserId = userId // Chỉ lấy items của user đang đăng nhập
                });

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ"));
            }
        }
    }
}
