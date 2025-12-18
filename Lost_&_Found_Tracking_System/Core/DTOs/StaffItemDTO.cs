namespace Core.DTOs
{
    public class StaffVerifyItemDTO
    {
        public string? Notes { get; set; }
    }

    public class StaffUpdateItemStatusDTO
    {
        public int NewStatusId { get; set; }
        public string? Notes { get; set; }
    }
}
