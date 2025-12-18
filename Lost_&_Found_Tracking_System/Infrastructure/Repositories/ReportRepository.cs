using Core.DTOs;
using Core.Interfaces;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ApplicationDBContext _context;

        private const int ITEM_PENDING = 1;
        private const int ITEM_CLAIMED = 3;
        private const int ITEM_RETURNED = 4;

        private const int CLAIM_PENDING = 1;
        private const int CLAIM_CONFLICT = 4;

        public ReportRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<StaffDashboardDTO> GetDashboardAsync(DateTime nowUtc)
        {
            if (nowUtc.Kind != DateTimeKind.Utc)
                nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

            var startOfDay = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 0, 0, DateTimeKind.Utc);
            var endOfDay = startOfDay.AddDays(1);

            var totalItems = await _context.Items.AsNoTracking().CountAsync();

            var itemsToday = await _context.Items.AsNoTracking()
                .CountAsync(i => i.CreatedAt >= startOfDay && i.CreatedAt < endOfDay);

            var pendingClaims = await _context.Claims.AsNoTracking()
                .CountAsync(c => c.StatusId == CLAIM_PENDING);

            var openConflicts = await _context.Claims.AsNoTracking()
                .CountAsync(c => c.StatusId == CLAIM_CONFLICT);

            var returned = await _context.Items.AsNoTracking()
                .CountAsync(i => i.StatusId == ITEM_RETURNED);

            var processing = Math.Max(0, totalItems - returned);

            var returnRate = await GetReturnRateThisMonthAsync(nowUtc);

            return new StaffDashboardDTO
            {
                ItemsReportedToday = itemsToday,
                PendingClaims = pendingClaims,
                TotalItems = totalItems,
                OpenConflicts = openConflicts,
                Summary = new StaffDashboardSummaryDTO
                {
                    TotalItems = totalItems,
                    Returned = returned,
                    Processing = processing
                },
                ReturnRate = returnRate
            };
        }

        public async Task<StaffReturnRateDTO> GetReturnRateThisMonthAsync(DateTime nowUtc)
        {
            if (nowUtc.Kind != DateTimeKind.Utc)
                nowUtc = DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

            var fromUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var toUtc = fromUtc.AddMonths(1);

            var claimed = await _context.Items.AsNoTracking()
                .CountAsync(i => i.StatusId == ITEM_CLAIMED
                              && i.UpdatedAt >= fromUtc
                              && i.UpdatedAt < toUtc);

            var returned = await _context.Items.AsNoTracking()
                .CountAsync(i => i.StatusId == ITEM_RETURNED
                              && i.UpdatedAt >= fromUtc
                              && i.UpdatedAt < toUtc);

            var denom = claimed + returned;
            var rate = denom == 0 ? 0 : (double)returned / denom;

            return new StaffReturnRateDTO
            {
                From = fromUtc,
                To = toUtc,
                Claimed = claimed,
                Returned = returned,
                Rate = rate
            };
        }



        public async Task<List<ItemExportRowDTO>> GetItemsForExportAsync(int take = 2000)
        {
            return await _context.Items.AsNoTracking()
                .Include(i => i.Campus)
                .Include(i => i.Status)
                .Include(i => i.ItemType)
                .OrderByDescending(i => i.CreatedAt)
                .Take(take)
                .Select(i => new ItemExportRowDTO
                {
                    ItemId = i.ItemId,
                    Title = i.Title,
                    Description = i.Description,
                    ItemType = i.ItemType != null ? i.ItemType.TypeName : null,
                    Status = i.Status != null ? i.Status.StatusName : null,
                    Campus = i.Campus != null ? i.Campus.CampusName : null,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }
    }
}
