using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Core.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("staff/reports")]
    [Authorize(Roles = "staff")]
    public class StaffReportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public StaffReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var nowUtc = DateTime.UtcNow;
            var dto = await _unitOfWork.ReportRepository.GetDashboardAsync(nowUtc);
            return Ok(dto);
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var items = await _unitOfWork.ReportRepository.GetItemsForExportAsync(2000);

            var sb = new StringBuilder();
            sb.AppendLine("item_id,title,description,item_type,status,campus,created_at,updated_at");

            foreach (var i in items)
            {
                string Esc(string? s) => string.IsNullOrEmpty(s) ? "" : '"' + s.Replace("\"", "\"\"") + '"';
                sb.AppendLine($"{i.ItemId},{Esc(i.Title)},{Esc(i.Description)},{Esc(i.ItemType)},{Esc(i.Status)},{Esc(i.Campus)},{i.CreatedAt:O},{i.UpdatedAt:O}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "fts_export.csv");
        }
    }
}
