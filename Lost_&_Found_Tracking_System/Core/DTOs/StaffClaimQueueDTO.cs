namespace Core.DTOs
{
    // For staff queue / processing
    public class StaffClaimQueueDTO
    {
        public int ClaimId { get; set; }
        public int ItemId { get; set; }
        public string? ItemTitle { get; set; }
        public string? ItemDescription { get; set; }
        public string? ItemImageUrl { get; set; }

        public Guid StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentEmail { get; set; }

        public string? ClaimDescription { get; set; }
        public string? EvidenceImageUrl { get; set; }
        public int StatusId { get; set; }
        public string? StatusName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class StaffVerifyClaimDTO
    {
        public int NewStatusId { get; set; }
        public string? Notes { get; set; }
    }

    public class StaffResolveConflictDTO
    {
        public int WinnerClaimId { get; set; }
        public string? Notes { get; set; }
    }
}
