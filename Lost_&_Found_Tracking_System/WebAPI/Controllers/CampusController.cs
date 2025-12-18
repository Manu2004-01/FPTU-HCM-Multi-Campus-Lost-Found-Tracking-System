using Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Errors;
using Core.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("Campuses")]
    [Authorize(Roles = "admin")]
    public class CampusController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampusController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var campuses = await _unitOfWork.CampusRepository.GetAllAsync();
            return Ok(campuses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCampus([FromBody] CreateCampusDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CampusName))
                return BadRequest(new BaseCommentResponse(400, "CampusName không hợp lệ"));

            var campus = await _unitOfWork.CampusRepository.CreateCampusAsync(dto);
            return Ok(campus);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCampus([FromRoute] int id, [FromBody] UpdateCampusDTO dto)
        {
            if (id <= 0) return BadRequest(new BaseCommentResponse(400, "CampusId không hợp lệ"));
            if (dto == null) return BadRequest(new BaseCommentResponse(400, "Thiếu dữ liệu"));

            var campus = await _unitOfWork.CampusRepository.UpdateCampusAsync(id, dto);
            if (campus == null)
                return NotFound(new BaseCommentResponse(404, "Không tìm thấy campus"));

            return Ok(campus);
        }
    }
}
