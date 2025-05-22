using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHDomain.Helpers;
using SHDomain.Models.Employees;
using SHServices.EmployeeService;

namespace StaffHR.Controllers
{
    public class EmployeeController(IEmployeeService _employeeService, IResponseHelper _responseHelper) : BaseController
    {
        [HttpGet("/api/employee/list")]
        public async Task<ActionResult> GetAllEmployeesAsync()
        {
            var response = await _employeeService.GetAllEmployeesAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/employee/search")]
        public async Task<ActionResult> SearchEmployeeAsync([FromQuery] EmployeeSearchModel searchModel)
        {
            var response = await _employeeService.SearchEmployeeAsync(searchModel);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/employee/create")]
        public async Task<ActionResult> CreateEmployeeAsync(Employee employee)
        {
            var response = await _employeeService.CreateEmployeeAsync(employee);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/employee/update/{id}")]
        public async Task<ActionResult> UpdateEmployeeAsync(int id, [FromBody] Employee employee)
        {
            if (id <= 0 || employee == null || employee.Id != id)
            {
                var err = _responseHelper.CreateResponse<Employee>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }
            var response = await _employeeService.UpdateEmployeeAsync(employee);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost("/api/employee/delete/{id}")]
        public async Task<ActionResult> DeleteEmployeeByIdAsync(int id)
        {
            var response = await _employeeService.DeleteEmployeeByIdAsync(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/employee/apartment-history/{id}")]
        public async Task<ActionResult> GetEmployeeApartmentHistory(int id)
        {
            var response = await _employeeService.GetEmployeeApartmentHistory(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/employee/vehicle-history/{id}")]
        public async Task<ActionResult> GetEmployeeVehicleHistory(int id)
        {
            var response = await _employeeService.GetEmployeeVehicleHistory(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/employee/travel-history/{id}")]
        public async Task<ActionResult> GetEmployeeTravelHistory(int id)
        {
            var response = await _employeeService.GetEmployeeTravelHistory(id);

            return StatusCode(response.StatusCode, response);
        }
    }
}
