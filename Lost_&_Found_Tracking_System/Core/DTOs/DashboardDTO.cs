using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class StaffDashboardDTO
    {
        public int ItemsReportedToday { get; set; }
        public int PendingClaims { get; set; }
        public int TotalItems { get; set; }
        public int OpenConflicts { get; set; }

        public StaffDashboardSummaryDTO Summary { get; set; } = new();
        public StaffReturnRateDTO ReturnRate { get; set; } = new();
    }

    public class StaffDashboardSummaryDTO
    {
        public int TotalItems { get; set; }
        public int Returned { get; set; }
        public int Processing { get; set; }
    }

    public class StaffReturnRateDTO
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Returned { get; set; }
        public int Claimed { get; set; }
        public double Rate { get; set; } // 0..1
    }

}
