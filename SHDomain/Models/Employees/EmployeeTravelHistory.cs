namespace SHDomain.Models.Employees
{
    public class EmployeeTravelHistory : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? DepartureDate { get; set; }
    }
}
