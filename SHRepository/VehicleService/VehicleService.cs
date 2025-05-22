using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.Employees;
using SHDomain.Models.Vehicle;

namespace SHServices.VehicleService
{
    public class VehicleService(IResponseHelper _responseHelper, AppDbContext _db, ILogger<VehicleService> _logger) : IVehicleService
    {
        #region START: CRUD Operations

        public async Task<ApiResponse<IEnumerable<Vehicle>>> GetAllVehiclesAsync()
        {
            try
            {
                var response = await _db.Vehicles.Where(v => v.IsDeleted == false).ToListAsync();

                return response.Count > 0
                    ? _responseHelper.CreateResponse<IEnumerable<Vehicle>>(true, 200, "Vehicles retrieved successfully", response)
                    : _responseHelper.CreateResponse<IEnumerable<Vehicle>>(false, 404, "Vehicles not found", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<IEnumerable<Vehicle>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Vehicle>> CreateVehicleAsync(Vehicle vehicle)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vehicle.Vin) ||
                    string.IsNullOrWhiteSpace(vehicle.LicensePlate) ||
                    string.IsNullOrWhiteSpace(vehicle.Make)
                    )
                {
                    return _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference.", null);
                }

                vehicle.LicensePlate = vehicle.LicensePlate.ToUpper();
                vehicle.Vin = vehicle.Vin.ToUpper();

                VehicleSearchModel search = new()
                {
                    Vin = vehicle.Vin,
                    LicensePlate = vehicle.LicensePlate
                };

                var searchResult = await SearchVehicleAsync(search);

                if (searchResult.StatusCode == 200)
                    return _responseHelper.CreateResponse<Vehicle>(false, 200, "Duplicate request", null);

                _db.Set<Vehicle>().Add(vehicle);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 201, "Vehicle created successfully.", vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Vehicle>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Vehicle>> UpdateVehicleAsync(Vehicle vehicle)
        {
            try
            {
                var existingVehicle = await _db.Vehicles.FirstAsync(prop => prop.Id == vehicle.Id);

                if (existingVehicle != null)
                {
                    await AuditVehicle(existingVehicle, vehicle);

                    existingVehicle.Vin = vehicle.Vin;
                    existingVehicle.LicensePlate = vehicle.LicensePlate;
                    existingVehicle.Make = vehicle.Make;
                    existingVehicle.RentalStartDate = vehicle.RentalStartDate;
                    existingVehicle.RentalEndDate = vehicle.RentalEndDate;
                    existingVehicle.EmployeeId = vehicle.EmployeeId;
                    existingVehicle.AgentId = vehicle.AgentId;
                    await _db.SaveChangesAsync();

                    return _responseHelper.CreateResponse(true, 200, "Vehicle updated successfully.", existingVehicle);
                }
                else
                {
                    return _responseHelper.CreateResponse<Vehicle>(false, 404, "Vehicle not found.", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Vehicle>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Vehicle>> SearchVehicleAsync(VehicleSearchModel searchModel)
        {
            try
            {
                if (searchModel.Id == null && string.IsNullOrWhiteSpace(searchModel.LicensePlate) && string.IsNullOrWhiteSpace(searchModel.Vin))
                    return _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference.", null);

                var query = _db.Vehicles.AsQueryable().Where(c => (c.Id == searchModel.Id || EF.Functions.Like(c.LicensePlate, searchModel.LicensePlate) || EF.Functions.Like(c.Vin, searchModel.Vin)) && c.IsDeleted == false);

                var result = await query.FirstOrDefaultAsync();

                return result != null
                    ? _responseHelper.CreateResponse(true, 200, "Vehicle retrieved successfully", result)
                    : _responseHelper.CreateResponse<Vehicle>(false, 404, "Vehicle not found", null);
            }
            catch (Exception ex)
            {
                return _responseHelper.CreateResponse<Vehicle>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<bool>> DeleteVehicleAsync(string vin)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(vin))
                    _responseHelper.CreateResponse<string>(false, 400, "Invalid request. Null object reference.", null);

                var result = await _db.Vehicles.FirstOrDefaultAsync(v => v.Vin.ToLower() == vin.ToLower());

                if (result == null)
                    return _responseHelper.CreateResponse(false, 404, "Vehicle not found.", false);

                result.IsDeleted = true;

                _db.Vehicles.Update(result);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 200, "Vehicle deleted successfully.", true);
            }
            catch (Exception ex)
            {
                return _responseHelper.CreateResponse(false, 500, ex.Message, false);
            }
        }

        #endregion END: CRUD Operations

        #region START: assign/unassign EMPLOYEE to/from vehicle

        public async Task<ApiResponse<Vehicle>> AssignEmployeeToVehicleAsync(int vehicleId, int employeeId)
        {
            return await SetVehicleAssignmentAsync(employeeId, vehicleId, assign: true);
        }

        public async Task<ApiResponse<Vehicle>> UnassignEmployeeFromVehicleAsync(int vehicleId, int employeeId)
        {
            return await SetVehicleAssignmentAsync(employeeId, vehicleId, assign: false);
        }

        private async Task<ApiResponse<Vehicle>> SetVehicleAssignmentAsync(int employeeId, int vehicleId, bool assign)
        {
            try
            {
                var vehicle = await _db.Vehicles.FirstOrDefaultAsync(c => c.Id == vehicleId);

                if (vehicle == null)
                    return _responseHelper.CreateResponse<Vehicle>(false, 404, "Vehicle not found", null);

                var employee = await _db.Employees.FirstOrDefaultAsync(c => c.Id == employeeId);

                vehicle.EmployeeId = assign ? employeeId : null;
                vehicle.AssignedTo = assign ? $"{employee?.FirstName} {employee?.LastName}" : "";
                await _db.SaveChangesAsync();

                if (assign)
                {
                    EmployeeVehicleHistory vehicleHistory = new()
                    {
                        EmployeeId = employeeId,
                        VehicleId = vehicleId,
                        RentalStartDate = vehicle.RentalStartDate,
                        RentalEndDate = vehicle.RentalEndDate,
                    };
                    _db.EmployeeVehicleHistory.Add(vehicleHistory);
                    await _db.SaveChangesAsync();
                }

                string message = assign ? "Vehicle assigned successfully" : "Vehicle unassigned successfully";

                return _responseHelper.CreateResponse(true, 200, message, vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Vehicle>(false, 500, ex.Message, null);
            }
        }

        #endregion END: assign/unassign EMPLOYEE to/from vehicle


        #region START: assign/unassign AGENT to/from vehicle

        public async Task<ApiResponse<Vehicle>> AssignAgentToVehicleAsync(int vehicleId, int agentId)
        {
            return await UpdateAgentAssignmentAsync(agentId, vehicleId, assign: true);
        }

        public async Task<ApiResponse<Vehicle>> UnassignAgentFromVehicleAsync(int vehicleId, int agentId)
        {
            return await UpdateAgentAssignmentAsync(agentId, vehicleId, assign: false);
        }

        private async Task<ApiResponse<Vehicle>> UpdateAgentAssignmentAsync(int agentId, int vehicleId, bool assign)
        {
            try
            {
                var vehicle = assign
                    ? await _db.Vehicles.FirstOrDefaultAsync(a => a.Id == vehicleId)
                    : await _db.Vehicles.FirstOrDefaultAsync(a => a.Id == vehicleId && a.AgentId == agentId);

                if (vehicle == null)
                    return _responseHelper.CreateResponse<Vehicle>(false, 404,
                        assign ? "Vehicle not found." : "Vehicle not found.", null);

                vehicle.AgentId = assign ? agentId : 0;
                await _db.SaveChangesAsync();

                var message = assign ? "Agent assigned successfully." : "Agent unassigned successfully.";

                return _responseHelper.CreateResponse(true, 200, message, vehicle);

                //var vehicle = await _db.Vehicles.FirstOrDefaultAsync(c => c.Id == vehicleId);

                //if (vehicle == null)
                //    return _responseHelper.CreateResponse<Vehicle>(false, 404, "Vehicle not found", null);

                //var employee = await _db.Employees.FirstOrDefaultAsync(c => c.Id == employeeId);

                //vehicle.EmployeeId = assign ? employeeId : null;
                //vehicle.AssignedTo = assign ? $"{employee?.FirstName} {employee?.LastName}" : "";
                //await _db.SaveChangesAsync();

                //string message = assign ? "Vehicle assigned successfully" : "Vehicle unassigned successfully";

                //return _responseHelper.CreateResponse(true, 200, message, vehicle);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Vehicle>(false, 500, ex.Message, null);
            }
        }

        #endregion

        private async Task AuditVehicle(Vehicle existingVehicle, Vehicle updatedVehicleModel)
        {
            List<Audit> changes = [];

            if (!existingVehicle.Vin.Equals(updatedVehicleModel.Vin, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Vehicles",
                    ColumnName = "Vin",
                    OldValue = existingVehicle.Vin,
                    NewValue = updatedVehicleModel.Vin,
                    Description = "The VIN of the vehicle is being updated.",
                    Created = DateTime.Now
                });
            }

            if (!existingVehicle.LicensePlate.Equals(updatedVehicleModel.LicensePlate, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Vehicles",
                    ColumnName = "LicensePlate",
                    OldValue = existingVehicle.LicensePlate,
                    NewValue = updatedVehicleModel.LicensePlate,
                    Description = "The license plate of the vehicle is being updated.",
                    Created = DateTime.Now
                });
            }

            if (!existingVehicle.Make.Equals(updatedVehicleModel.Make, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Vehicles",
                    ColumnName = "Make",
                    OldValue = existingVehicle.Make,
                    NewValue = updatedVehicleModel.Make,
                    Description = "The make of the vehicle is being updated.",
                    Created = DateTime.Now
                });
            }

            if (existingVehicle.RentalStartDate != updatedVehicleModel.RentalStartDate)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Vehicles",
                    ColumnName = "RentalStartDate",
                    OldValue = existingVehicle.RentalStartDate?.ToString("o"),
                    NewValue = updatedVehicleModel.RentalStartDate?.ToString("o"),
                    Description = "The rental start date of the vehicle is being updated.",
                    Created = DateTime.Now
                });
            }

            if (existingVehicle.RentalEndDate != updatedVehicleModel.RentalEndDate)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Vehicles",
                    ColumnName = "RentalEndDate",
                    OldValue = existingVehicle.RentalEndDate?.ToString("o"),
                    NewValue = updatedVehicleModel.RentalEndDate?.ToString("o"),
                    Description = "The rental end date of the vehicle is being updated.",
                    Created = DateTime.Now
                });
            }

            if (changes.Count != 0)
            {
                _db.Audits.AddRange(changes);
                await _db.SaveChangesAsync();
            }
        }
    }
}
