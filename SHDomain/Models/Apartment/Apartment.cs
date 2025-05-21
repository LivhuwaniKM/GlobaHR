namespace SHDomain.Models.Apartment
{
    public class Apartment : BaseEntity
    {
        public string Details { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int Rooms { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public string? AssignedTo { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public int? AgentId { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}