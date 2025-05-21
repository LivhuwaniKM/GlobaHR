using SHDomain.Models;
using SHDomain.Models.Apartment;
using SHDomain.Models.Employees;
using SHDomain.Models.Vehicle;

namespace SHServices.EmployeeService
{
    public interface IEmployeeService
    {
        Task<ApiResponse<IEnumerable<Employee>>> GetAllEmployeesAsync();
        Task<ApiResponse<Employee>> SearchEmployeeAsync(EmployeeSearchModel searchModel);
        Task<ApiResponse<Employee>> CreateEmployeeAsync(Employee employee);
        Task<ApiResponse<Employee>> UpdateEmployeeAsync(Employee employee);
        Task<ApiResponse<string>> DeleteEmployeeByIdAsync(int employeeId);
        Task<ApiResponse<Apartment[]>> GetEmployeeApartmentHistory(int employeeId);
        Task<ApiResponse<Vehicle[]>> GetEmployeeVehicleHistory(int employeeId);
        Task<ApiResponse<List<EmployeeTravelHistory>>> GetEmployeeTravelHistory(int employeeId);
    }
}
