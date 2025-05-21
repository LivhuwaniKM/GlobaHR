using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHDomain.Helpers;
using SHDomain.Models.Apartment;
using SHServices.ApartmentService;

namespace StaffHR.Controllers
{
    public class ApartmentController(IResponseHelper _responseHelper, IApartmentService _apartmentService) : BaseController
    {
        [HttpGet("/api/apartment/get/all")]
        public async Task<ActionResult> GetAllApartmentsAsync()
        {
            var response = await _apartmentService.GetAllApartmentsAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/apartment/search")]
        public async Task<ActionResult> GetApartmentBySearchAsync([FromQuery] ApartmentSearchModel searchModel)
        {
            var response = await _apartmentService.GetApartmentBySearchAsync(searchModel);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/apartment/update/{apartmentId}")]
        public async Task<ActionResult> UpdateApartmentAsync(int apartmentId, [FromBody] Apartment apartment)
        {
            if (apartment == null || apartmentId != apartment.Id)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }
            var response = await _apartmentService.UpdateApartmentAsync(apartment);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/apartment/create")]
        public async Task<ActionResult> CreateApartmentAsync(Apartment apartment)
        {
            var response = await _apartmentService.CreateApartmentAsync(apartment);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost("/api/apartment/delete/{apartmentId}")]
        public async Task<ActionResult> DeleteApartmentAsync(int apartmentId)
        {
            var response = await _apartmentService.DeleteApartmentByIdAsync(apartmentId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/apartment/assign-employee")]
        public async Task<ActionResult> AssignEmployeeToApartmentAsync(ApartmenAssignmentRequest request)
        {
            if (request == null || request.EmployeeId <= 0 || request.ApartmentId <= 0)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }

            var response = await _apartmentService.AssignEmployeeToApartmentAsync(request.ApartmentId, request.EmployeeId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/apartment/unassign-employee")]
        public async Task<ActionResult> UnassignEmployeeFromApartmentAsync(ApartmenAssignmentRequest request)
        {
            if (request == null || request.EmployeeId <= 0 || request.ApartmentId <= 0)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }

            var response = await _apartmentService.UnassignEmployeeFromApartmentAsync(request.ApartmentId, request.EmployeeId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/apartment/assign-agent")]
        public async Task<ActionResult> AssignAgentToApartmentAsync(int apartmentId, int agentId)
        {
            if (apartmentId <= 0 || agentId <= 0)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid apartment or agent ID.", null);
                return StatusCode(err.StatusCode, err);
            }

            var response = await _apartmentService.AssignAgentToApartmentAsync(apartmentId, agentId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/apartment/unassign-agent")]
        public async Task<ActionResult> UnassignAgentFromApartmentAsync(int apartmentId, int agentId)
        {
            if (apartmentId <= 0 || agentId <= 0)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid apartment or agent ID.", null);
                return StatusCode(err.StatusCode, err);
            }

            var response = await _apartmentService.UnassignAgentFromApartmentAsync(apartmentId, agentId);
            return StatusCode(response.StatusCode, response);
        }
    }
}