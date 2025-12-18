using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class StaffReceiveItemDTO
    {
        public int ItemId { get; set; }
        public string? Notes { get; set; }
    }

    public class StaffScheduleReturnDTO
    {
        public int ItemId { get; set; }
        public Guid StudentId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
    }

    public class StaffReturnItemDTO
    {
        public int ItemId { get; set; }
        public Guid StudentId { get; set; }
        public IFormFile? SignatureImage { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateAppointmentResult
    {
        public bool Ok { get; set; }
        public int AppointmentId { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
