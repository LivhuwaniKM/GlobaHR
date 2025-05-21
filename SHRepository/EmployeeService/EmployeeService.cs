using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.Apartment;
using SHDomain.Models.Employees;
using SHDomain.Models.Vehicle;

namespace SHServices.EmployeeService
{
    public class EmployeeService(IResponseHelper _responseHelper, ILogger<EmployeeService> _logger, AppDbContext _db) : IEmployeeService
    {
        public async Task<ApiResponse<IEnumerable<Employee>>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _db.Employees.Where(a => a.IsDeleted == false).ToListAsync();

                return (employees.Count > 0)
                    ? _responseHelper.CreateResponse<IEnumerable<Employee>>(true, 200, "Employees retrieved successfully.", employees)
                    : _responseHelper.CreateResponse<IEnumerable<Employee>>(false, 404, "Employees not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<IEnumerable<Employee>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Employee>> SearchEmployeeAsync(EmployeeSearchModel searchModel)
        {
            if (searchModel.EmployeeId <= 0 && string.IsNullOrWhiteSpace(searchModel.Email))
                return _responseHelper.CreateResponse<Employee>(false, 400, "Invalid request. Null object reference", null);

            try
            {
                var query = _db.Employees.AsQueryable().Where(c => c.Id == searchModel.EmployeeId || c.Email == searchModel.Email);

                var result = await query.FirstOrDefaultAsync();

                return result != null
                    ? _responseHelper.CreateResponse(true, 200, "Employee retrieved successfully.", result)
                    : _responseHelper.CreateResponse<Employee>(false, 404, "Employee not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Employee>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Employee>> CreateEmployeeAsync(Employee employee)
        {
            if (
                string.IsNullOrWhiteSpace(employee.FirstName) ||
                string.IsNullOrWhiteSpace(employee.LastName) ||
                string.IsNullOrWhiteSpace(employee.Email)
                )
            {
                return _responseHelper.CreateResponse<Employee>(false, 400, "Invalid request. Null object reference", null);
            }

            try
            {
                var response = await _db.Employees.FirstOrDefaultAsync(prop => prop.Email.ToLower() == employee.Email.ToLower());

                if (response != null)
                    return _responseHelper.CreateResponse<Employee>(true, 200, "Duplicate record found.", null);

                _db.Employees.Add(employee);
                await _db.SaveChangesAsync();

                if (employee.ArrivalDate != null && employee.DepartureDate != null)
                {
                    EmployeeTravelHistory history = new()
                    {
                        EmployeeId = employee.Id,
                        ArrivalDate = employee.ArrivalDate,
                        DepartureDate = employee.DepartureDate
                    };
                    _db.EmployeeTravelHistory.Add(history);
                    await _db.SaveChangesAsync();
                }

                return _responseHelper.CreateResponse(true, 201, "Employee created successfully.", employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Employee>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Employee>> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                var existingEmployee = await _db.Employees.FirstOrDefaultAsync(prop => prop.Id == employee.Id);

                if (existingEmployee != null)
                {
                    await AuditEmployeeAsync(existingEmployee, employee);

                    existingEmployee.FirstName = employee.FirstName;
                    existingEmployee.LastName = employee.LastName;
                    existingEmployee.Email = employee.Email;
                    existingEmployee.ArrivalDate = employee.ArrivalDate;
                    existingEmployee.DepartureDate = employee.DepartureDate;
                    existingEmployee.PassportId = employee.PassportId;

                    _db.Entry(existingEmployee).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    if (employee.ArrivalDate != null && employee.DepartureDate != null)
                    {
                        EmployeeTravelHistory history = new()
                        {
                            EmployeeId = employee.Id,
                            ArrivalDate = employee.ArrivalDate,
                            DepartureDate = employee.DepartureDate
                        };
                        _db.EmployeeTravelHistory.Add(history);
                        await _db.SaveChangesAsync();
                    }

                    return _responseHelper.CreateResponse(true, 200, "Employee updated successfully.", existingEmployee);
                }
                else
                {
                    return _responseHelper.CreateResponse<Employee>(false, 404, "Employee not found.", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Employee>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<string>> DeleteEmployeeByIdAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    return _responseHelper.CreateResponse<string>(false, 400, "Invalid request. Null object reference", null);

                var employee = await _db.Employees.FirstOrDefaultAsync(prop => prop.Id == employeeId);

                if (employee != null)
                {
                    employee.IsDeleted = true;

                    _db.Employees.Update(employee);
                    await _db.SaveChangesAsync();

                    return _responseHelper.CreateResponse<string>(true, 200, "Employee deleted successfully.", null);
                }
                return _responseHelper.CreateResponse<string>(false, 404, "Employee not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<string>(false, 500, ex.Message, null);
            }
        }

        private async Task AuditEmployeeAsync(Employee existingEmployee, Employee updatedEmployeeModel)
        {
            List<Audit> changes = [];

            if (!existingEmployee.FirstName.Equals(updatedEmployeeModel.FirstName, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Employee",
                    ColumnName = "FirstName",
                    OldValue = existingEmployee.FirstName,
                    NewValue = updatedEmployeeModel.FirstName,
                    Description = "An update of first name on employee table",
                    Created = DateTime.Now,
                });
            }

            if (!existingEmployee.LastName.Equals(updatedEmployeeModel.LastName, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Employee",
                    ColumnName = "LastName",
                    OldValue = existingEmployee.LastName,
                    NewValue = updatedEmployeeModel.LastName,
                    Description = "An update of last name on employee table",
                    Created = DateTime.Now,
                });
            }

            if (!existingEmployee.Email.Equals(updatedEmployeeModel.Email, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Employee",
                    ColumnName = "Email",
                    OldValue = existingEmployee.Email,
                    NewValue = updatedEmployeeModel.Email,
                    Description = "An update of email on employee table",
                    Created = DateTime.Now,
                });
            }

            if (existingEmployee.DepartureDate != updatedEmployeeModel.DepartureDate)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Employee",
                    ColumnName = "DepartureDate",
                    OldValue = existingEmployee.DepartureDate?.ToString("o"),
                    NewValue = updatedEmployeeModel.DepartureDate?.ToString("o"),
                    Description = "An update of departure date on employee table",
                    Created = DateTime.Now,
                });
            }

            if (changes.Count != 0)
            {
                _db.Audits.AddRange(changes);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<ApiResponse<Apartment[]>> GetEmployeeApartmentHistory(int employeeId)
        {
            var historyRecords = await _db.EmployeeApartmentHistory
                    .Where(c => c.EmployeeId == employeeId)
                    .ToListAsync();

            var apartmentIds = historyRecords.Select(h => h.ApartmentId).ToList();

            var apartmentsList = await _db.Apartments
                    .Where(a => apartmentIds.Contains(a.Id))
                    .ToArrayAsync();

            return apartmentsList.Any()
                ? _responseHelper.CreateResponse(true, 200, "Apartments history retrieved successfully", apartmentsList)
                : _responseHelper.CreateResponse<Apartment[]>(false, 404, "Apartments history not found", null);
        }

        public async Task<ApiResponse<Vehicle[]>> GetEmployeeVehicleHistory(int employeeId)
        {
            var historyRecords = await _db.EmployeeVehicleHistory
                .Where(c => c.EmployeeId == employeeId)
                .ToArrayAsync();

            var vehicleIds = historyRecords.Select(h => h.VehicleId).ToList();

            var vehiclesList = await _db.Vehicles
                .Where(c => vehicleIds.Contains(c.Id))
                .ToArrayAsync();

            return vehiclesList.Any()
                ? _responseHelper.CreateResponse(true, 200, "Vehicles history retrieved successfully", vehiclesList)
                : _responseHelper.CreateResponse<Vehicle[]>(false, 404, "Vehicles history not found", null);
        }

        public async Task<ApiResponse<List<EmployeeTravelHistory>>> GetEmployeeTravelHistory(int employeeId)
        {
            var response = await _db.EmployeeTravelHistory.Where(c => c.EmployeeId == employeeId).ToListAsync();

            return response != null
                ? _responseHelper.CreateResponse(true, 200, "Travel history retrieved successfully", response)
                : _responseHelper.CreateResponse<List<EmployeeTravelHistory>>(false, 404, "Travel history not found", null);
        }
    }
}
