using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.Apartment;

namespace SHServices.ApartmentService
{
    public class ApartmentService(IResponseHelper _responseHelper, AppDbContext _db, ILogger<ApartmentService> _logger) : IApartmentService
    {
        #region START: Apartment CRUD Operations
        public async Task<ApiResponse<IEnumerable<Apartment>>> GetAllApartmentsAsync()
        {
            try
            {
                var apartments = await _db.Apartments.Where(a => a.IsDeleted == false).ToListAsync();

                return (apartments.Count > 0)
                    ? _responseHelper.CreateResponse<IEnumerable<Apartment>>(true, 200, "Apartments retrieved successfully.", apartments)
                    : _responseHelper.CreateResponse<IEnumerable<Apartment>>(false, 404, "Apartments not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<IEnumerable<Apartment>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Apartment>> GetApartmentBySearchAsync(ApartmentSearchModel searchModel)
        {
            try
            {
                if (searchModel.ApartmentId == null && searchModel.EmployeeId == null)
                {
                    return _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference", null);
                }

                var query = _db.Apartments.AsQueryable().Where(c => c.Id == searchModel.ApartmentId || c.EmployeeId == searchModel.EmployeeId);

                var result = await query.FirstOrDefaultAsync();

                return result != null
                    ? _responseHelper.CreateResponse(true, 200, "Apartment retrieved successfully.", result)
                    : _responseHelper.CreateResponse<Apartment>(false, 404, "Apartment not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Apartment>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Apartment>> UpdateApartmentAsync(Apartment apartment)
        {
            try
            {
                var response = await _db.Apartments.FirstOrDefaultAsync(prop => prop.Id == apartment.Id);
                if (response == null)
                {
                    return _responseHelper.CreateResponse<Apartment>(false, 404, "Apartment not found.", null);
                }

                await AuditApartment(response, apartment);

                response.Details = apartment.Details;
                response.Address = apartment.Address;
                response.RentalStartDate = apartment.RentalStartDate;
                response.RentalEndDate = apartment.RentalEndDate;
                response.EmployeeId = apartment.EmployeeId;

                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 200, "Apartment updated successfully.", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating apartment.");
                return _responseHelper.CreateResponse<Apartment>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Apartment>> CreateApartmentAsync(Apartment apartment)
        {
            try
            {
                if (apartment == null || string.IsNullOrWhiteSpace(apartment.Details) || string.IsNullOrWhiteSpace(apartment.Address))
                    return _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference.", null);

                _db.Set<Apartment>().Add(apartment);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 201, "Apartment created successfully.", apartment);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Apartment>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<string>> DeleteApartmentByIdAsync(int apartmentId)
        {
            try
            {
                if (apartmentId <= 0)
                    return _responseHelper.CreateResponse<string>(false, 400, "Invalid request. Null object referenece", null);

                var response = (from ap in _db.Apartments where ap.Id == apartmentId select ap).Single();

                if (response != null)
                {
                    response.IsDeleted = true;

                    _db.Apartments.Update(response);
                    await _db.SaveChangesAsync();

                    return _responseHelper.CreateResponse<string>(true, 200, "Apartment deleted successfully.", null);
                }
                return _responseHelper.CreateResponse<string>(false, 404, "Apartment not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<string>(false, 500, ex.Message, null);
            }
        }

        #endregion END: Apartment CRUD Operations

        #region START: assign/unassign EMPLOYEE to/from apartment
        public async Task<ApiResponse<Apartment>> AssignEmployeeToApartmentAsync(int apartmentId, int employeeId)
        {
            return await SetApartmentAssignmentAsync(apartmentId, employeeId, assign: true);
        }

        public async Task<ApiResponse<Apartment>> UnassignEmployeeFromApartmentAsync(int apartmentId, int employeeId)
        {
            return await SetApartmentAssignmentAsync(apartmentId, employeeId, assign: false);
        }

        private async Task<ApiResponse<Apartment>> SetApartmentAssignmentAsync(int apartmentId, int employeeId, bool assign)
        {
            try
            {
                var apartment = await _db.Apartments.FirstOrDefaultAsync(c => c.Id == apartmentId);
                if (apartment == null)
                    return _responseHelper.CreateResponse<Apartment>(false, 404, "Apartment not found", null);

                var employee = assign
                    ? await _db.Employees.FirstOrDefaultAsync(c => c.Id == employeeId)
                    : null;

                apartment.EmployeeId = assign ? employeeId : null;
                apartment.AssignedTo = assign && employee != null
                    ? $"{employee.FirstName} {employee.LastName}"
                    : string.Empty;

                await _db.SaveChangesAsync();

                var message = assign ? "Apartment assigned successfully" : "Apartment unassigned successfully";
                return _responseHelper.CreateResponse(true, 200, message, apartment);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Apartment>(false, 500, ex.Message, null);
            }
        }

        #endregion END: assign/unassign EMPLOYEE to/from apartment

        #region START: assign/unassign AGENT to/from apartment

        public async Task<ApiResponse<Apartment>> AssignAgentToApartmentAsync(int apartmentId, int agentId)
        {
            return await UpdateAgentAssignmentAsync(apartmentId, agentId, assign: true);
        }

        public async Task<ApiResponse<Apartment>> UnassignAgentFromApartmentAsync(int apartmentId, int agentId)
        {
            return await UpdateAgentAssignmentAsync(apartmentId, agentId, assign: false);
        }

        private async Task<ApiResponse<Apartment>> UpdateAgentAssignmentAsync(int apartmentId, int agentId, bool assign)
        {
            var apartment = assign
                ? await _db.Apartments.FirstOrDefaultAsync(a => a.Id == apartmentId)
                : await _db.Apartments.FirstOrDefaultAsync(a => a.Id == apartmentId && a.AgentId == agentId);

            if (apartment == null)
                return _responseHelper.CreateResponse<Apartment>(false, 404,
                    assign ? "Apartment not found." : "Apartment not found or agent not assigned.", null);

            apartment.AgentId = assign ? agentId : null;
            await _db.SaveChangesAsync();

            var message = assign ? "Agent assigned successfully." : "Agent unassigned successfully.";
            return _responseHelper.CreateResponse(true, 200, message, apartment);
        }

        #endregion END: assign/unassign AGENT to/from apartment

        private async Task AuditApartment(Apartment existingApartment, Apartment updatedApartmentModel)
        {
            List<Audit> changes = [];

            if (!existingApartment.Details.Equals(updatedApartmentModel.Details, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Apartments",
                    ColumnName = "Details",
                    OldValue = existingApartment.Details,
                    NewValue = updatedApartmentModel.Details,
                    Description = "A detailed description of the apartment is being updated.",
                    Created = DateTime.Now
                });
            }

            if (!existingApartment.Address.Equals(updatedApartmentModel.Address, StringComparison.CurrentCultureIgnoreCase))
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Apartments",
                    ColumnName = "Address",
                    OldValue = existingApartment.Address,
                    NewValue = updatedApartmentModel.Address,
                    Description = "The address of the apartment is being updated.",
                    Created = DateTime.Now
                });
            }

            if (existingApartment.RentalEndDate != updatedApartmentModel.RentalEndDate)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Apartments",
                    ColumnName = "RentalEndDate",
                    OldValue = existingApartment.RentalEndDate?.ToString("o"),
                    NewValue = updatedApartmentModel.RentalEndDate?.ToString("o"),
                    Description = "The rental end date of the apartment is being updated.",
                    Created = DateTime.Now
                });
            }

            if (existingApartment.RentalStartDate != updatedApartmentModel.RentalStartDate)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Apartments",
                    ColumnName = "RentalStartDate",
                    OldValue = existingApartment.RentalStartDate?.ToString("o"),
                    NewValue = updatedApartmentModel.RentalStartDate?.ToString("o"),
                    Description = "The rental start date of the apartment is being updated.",
                    Created = DateTime.Now
                });
            }

            if (existingApartment.EmployeeId != updatedApartmentModel.EmployeeId)
            {
                changes.Add(new Audit
                {
                    EmployeeId = "employeeSessionId",
                    OperationName = "Update",
                    TableName = "Apartments",
                    ColumnName = "EmployeeId",
                    OldValue = existingApartment.EmployeeId.ToString(),
                    NewValue = updatedApartmentModel.EmployeeId.ToString(),
                    Description = "The employee responsible for the apartment is being updated.",
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
