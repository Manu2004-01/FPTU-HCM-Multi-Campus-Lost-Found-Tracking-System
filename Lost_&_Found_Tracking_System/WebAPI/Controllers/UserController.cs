using AutoMapper;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.Errors;

namespace WebAPI.Controllers
{
    [Route("User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("profile")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status401Unauthorized)]
        [ResponseType(typeof(BaseCommentResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"Extracted userIdClaim: {userIdClaim}");
                
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    Console.WriteLine("Failed to parse userIdClaim as Guid");
                    return Unauthorized(new BaseCommentResponse(401, "Token không hợp lệ hoặc không chứa thông tin người dùng"));
                }

                Console.WriteLine($"Looking for user with userId: {userId}");
                var user = await _unitOfWork.UserRepository.GetProfileAsync(userId);
                if (user == null)
                {
                    Console.WriteLine($"User not found with userId: {userId}");
                    return NotFound(new BaseCommentResponse(404, "Không tìm thấy thông tin người dùng"));
                }
                
                Console.WriteLine($"User found: {user.Email}");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommentResponse(500, "Lỗi máy chủ: " + ex.Message));
            }
        }
    }
}
