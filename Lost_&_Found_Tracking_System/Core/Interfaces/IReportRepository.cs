using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IReportRepository
    {
        Task<StaffDashboardDTO> GetDashboardAsync(DateTime nowUtc);
        Task<StaffReturnRateDTO> GetReturnRateThisMonthAsync(DateTime nowUtc);
        Task<List<ItemExportRowDTO>> GetItemsForExportAsync(int take = 2000);
    }

    public class ItemExportRowDTO
    {
        public int ItemId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ItemType { get; set; }
        public string? Status { get; set; }
        public string? Campus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
