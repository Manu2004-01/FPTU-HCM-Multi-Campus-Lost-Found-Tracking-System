using Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Errors;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("staff/handover")]
    [Authorize(Roles = "staff")]
    public class StaffHandoverController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public StaffHandoverController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        [HttpPost("receive")]
        public async Task<IActionResult> Receive([FromBody] StaffReceiveItemDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (dto == null || dto.ItemId <= 0)
                return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));

            var ok = await _unitOfWork.ItemRepository.ReceiveItemAsync(dto.ItemId, staffId.Value, dto.Notes);
            if (!ok)
                return NotFound(new BaseCommentResponse(404, "Không tìm thấy item hoặc không thể nhận đồ"));

            return Ok(new BaseCommentResponse(200, "Nhận đồ thành công"));
        }

        // Tạo lịch hẹn trả đồ
        [HttpPost("schedule")]
        public async Task<IActionResult> Schedule([FromBody] StaffScheduleReturnDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (dto == null || dto.ItemId <= 0)
                return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));

            var result = await _unitOfWork.AppointmentRepository.CreateReturnAppointmentAsync(
                staffId.Value, dto.ItemId, dto.StudentId, dto.Date, dto.Time);

            if (!result.Ok)
                return NotFound(new BaseCommentResponse(404, result.ErrorMessage ?? "Không thể tạo lịch hẹn"));

            return Ok(new { message = "Tạo lịch hẹn thành công", appointmentId = result.AppointmentId });
        }

        // Trả đồ + lưu chữ ký (multipart/form-data)
        [HttpPost("return")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ReturnItem([FromForm] StaffReturnItemDTO dto)
        {
            var staffId = GetUserIdFromToken();
            if (staffId == null)
                return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));

            if (dto == null || dto.ItemId <= 0)
                return BadRequest(new BaseCommentResponse(400, "ItemId không hợp lệ"));

            // lưu file chữ ký (nếu có) => ra URL
            var signatureUrl = await SaveSignatureIfProvided(dto.SignatureImage);

            var ok = await _unitOfWork.ItemRepository.ReturnItemAsync(
                itemId: dto.ItemId,
                staffId: staffId.Value,
                studentId: dto.StudentId,
                signatureUrl: signatureUrl,
                notes: dto.Notes
            );

            if (!ok)
                return NotFound(new BaseCommentResponse(404, "Không tìm thấy item hoặc không thể trả đồ"));

            return Ok(new { message = "Trả đồ thành công", signatureUrl });
        }

        private async Task<string?> SaveSignatureIfProvided(IFormFile? signatureImage)
        {
            if (signatureImage == null) return null;

            var ext = Path.GetExtension(signatureImage.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var folder = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "signatures");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, fileName);
            using (var stream = System.IO.File.Create(filePath))
            {
                await signatureImage.CopyToAsync(stream);
            }

            return $"/images/signatures/{fileName}";
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                              ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(userIdClaim, out var id) ? id : null;
        }
    }
}
