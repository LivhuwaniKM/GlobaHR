namespace SHDomain.Models.Employees
{
    public class EmployeeVehicleHistory : BaseEntity
    {
        public int? EmployeeId { get; set; }
        public int? VehicleId { get; set; }
        public DateTime? RentalStartDate { get; set; }
        public DateTime? RentalEndDate { get; set; }
    }
}
