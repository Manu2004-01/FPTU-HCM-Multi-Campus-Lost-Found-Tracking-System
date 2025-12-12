using Core.DTOs;
using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Errors;

namespace WebAPI.Controllers
{
    [Route("Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDBContext _db;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDBContext db, ITokenService tokenService, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _db = db;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromQuery] AuthDTO login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));

                if (login == null)
                    return BadRequest(new BaseCommentResponse(400, "Thiếu thông tin đăng nhập"));

                if (login.CampusID <= 0)
                    return BadRequest(new BaseCommentResponse(400, "CampusID là bắt buộc và phải lớn hơn 0"));

                if (string.IsNullOrWhiteSpace(login.Email))
                    return BadRequest(new BaseCommentResponse(400, "Email là bắt buộc"));

                if (string.IsNullOrWhiteSpace(login.Password))
                    return BadRequest(new BaseCommentResponse(400, "Mật khẩu là bắt buộc"));

                var user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == login.Email && u.CampusId == login.CampusID);

                if (user == null)
                    return Unauthorized(new BaseCommentResponse(401, "Thông tin đăng nhập không hợp lệ"));

                // Kiểm tra mật khẩu hash
                if (!PasswordHelper.VerifyPassword(login.Password, user.PasswordHash))
                    return Unauthorized(new BaseCommentResponse(401, "Thông tin đăng nhập không hợp lệ"));

                // Ensure Role is loaded
                if (user.Role == null)
                {
                    await _db.Entry(user).Reference(u => u.Role).LoadAsync();
                }

                var roleName = user.Role?.RoleName ?? string.Empty;
                var token = _tokenService.CreateToken(user, roleName);
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                // Log exception for debugging
                Console.WriteLine($"Login error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ trong quá trình đăng nhập"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        [ResponseType(StatusCodes.Status204NoContent)]
        [ResponseType(StatusCodes.Status401Unauthorized)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ"));
                }
                return Ok(new BaseCommentResponse(200, "Đăng xuất thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ trong quá trình đăng xuất"));
            }
        }

        [HttpPost("register")]
        [ResponseType(StatusCodes.Status201Created)]
        [ResponseType(StatusCodes.Status400BadRequest)]
        [ResponseType(StatusCodes.Status409Conflict)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromQuery] RegisterDTO register)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new BaseCommentResponse(400, "Dữ liệu đầu vào không hợp lệ"));
                if (register == null)
                    return BadRequest(new BaseCommentResponse(400, "Thiếu thông tin đăng ký"));

                if (!register.CampusId.HasValue || register.CampusId.Value <= 0)
                    return BadRequest(new BaseCommentResponse(400, "CampusId là bắt buộc và phải lớn hơn 0"));

                if (string.IsNullOrWhiteSpace(register.FullName))
                    return BadRequest(new BaseCommentResponse(400, "FullName là bắt buộc"));

                if (string.IsNullOrWhiteSpace(register.Email))
                    return BadRequest(new BaseCommentResponse(400, "Email là bắt buộc"));

                if (string.IsNullOrWhiteSpace(register.PasswordHash))
                    return BadRequest(new BaseCommentResponse(400, "Password là bắt buộc"));
                var existingUser = await _db.Users
                    .AnyAsync(u => u.Email == register.Email && u.CampusId == register.CampusId);
                if (existingUser)
                    return Conflict(new BaseCommentResponse(409, "Người dùng với email này đã tồn tại"));
                // Hash mật khẩu trước khi lưu
                var hashedPassword = PasswordHelper.HashPassword(register.PasswordHash);

                var newUser = new Core.Entities.User
                {
                    UserId = Guid.NewGuid(),
                    Fullname = register.FullName,
                    Email = register.Email,
                    CampusId = register.CampusId,
                    PasswordHash = hashedPassword,
                    // user_role enum trên DB: student | staff | security
                    RoleId = 1, // student role_id = 1
                    Phone = string.Empty,
                    CreateAt = DateTime.UtcNow,
                    UpdateAt = DateTime.UtcNow
                };
                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return StatusCode(201, new BaseCommentResponse(201, "Đăng ký thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ trong quá trình đăng ký"));
            }
        }
    }
}
