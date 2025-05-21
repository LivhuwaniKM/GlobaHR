namespace SHDomain.Models.Vehicle
{
    public class Vehicle : BaseEntity
    {
        public string Vin { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
        public string? AssignedTo { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public int? AgentId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}