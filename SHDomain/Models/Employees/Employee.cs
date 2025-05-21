namespace SHDomain.Models.Employees
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string PassportId { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
    }
}