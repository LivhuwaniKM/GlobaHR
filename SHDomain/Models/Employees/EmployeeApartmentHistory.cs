namespace SHDomain.Models.Employees
{
    public class EmployeeApartmentHistory : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public int? ApartmentId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
    }
}
