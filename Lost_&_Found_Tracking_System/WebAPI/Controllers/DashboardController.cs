using AutoMapper;
using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Errors;
using WebAPI.Helpers;

namespace WebAPI.Controllers
{
    [Route("Dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("dashboard")]
        [Authorize]
        [ResponseType(StatusCodes.Status200OK)]
        [ResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboard([FromQuery] int pagenumber, [FromQuery] int pazesize, [FromQuery] string? sort = null, [FromQuery] int? itemTypeId = null, [FromQuery] int? statusId = null, [FromQuery] int? categoryId = null, [FromQuery] int? campusId = null, [FromQuery] string? search = null)
        {
            try
            {
                if (pagenumber <= 0 || pazesize <= 0)
                    return BadRequest(new BaseCommentResponse(400, "Số trang và kích thước trang phải lớn hơn 0"));

                var dashboardData = await _unitOfWork.ItemRepository.GetItemDashboardAsync(new Core.Sharing.EntityParam
                {
                    Pagenumber = pagenumber,
                    Pagesize = pazesize,
                    Sorting = sort,
                    Search = search,
                    ItemTypeId = itemTypeId,
                    StatusId = statusId,
                    CategoryId = categoryId,
                    CampusId = campusId,
                });

                var totalIteams = await _unitOfWork.ItemRepository.CountAsync();

                var result = _mapper.Map<List<ItemDTO>>(dashboardData);

                return Ok(new Pagination<ItemDTO>(pazesize, pagenumber, totalIteams, result));
            }
            catch 
            { 
                return StatusCode(500, new BaseCommentResponse(500, "Đã xảy ra lỗi máy chủ nội bộ"));
            }
        }
    }
}
