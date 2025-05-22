using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHDomain.Helpers;
using SHDomain.Models.Agent;
using SHDomain.Models.Vehicle;
using SHServices.VehicleService;

namespace StaffHR.Controllers
{
    public class VehicleController(IResponseHelper _responseHelper, IVehicleService _vehicleService) : BaseController
    {
        [HttpGet("/api/vehicle/list")]
        public async Task<ActionResult> GetAllVehicles()
        {
            var response = await _vehicleService.GetAllVehiclesAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/vehicle/update/{id}")]
        public async Task<ActionResult> UpdateVehicleAsync(int id, [FromBody] Vehicle vehicle)
        {
            if (id <= 0 || vehicle == null || id != vehicle.Id)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference.", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.UpdateVehicleAsync(vehicle);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/create")]
        public async Task<ActionResult> CreateVehicleAsync(Vehicle vehicle)
        {
            var response = await _vehicleService.CreateVehicleAsync(vehicle);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/vehicle/search")]
        public async Task<ActionResult> SearchVehicleAsync([FromQuery] VehicleSearchModel searchModel)
        {
            var response = await _vehicleService.SearchVehicleAsync(searchModel);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost("/api/vehicle/delete/{id}")]
        public async Task<ActionResult> DeleteVehicleAsync(string id)
        {
            var response = await _vehicleService.DeleteVehicleAsync(id);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/assign-employee")]
        public async Task<ActionResult> AssignEmployeeToVehicleAsync(VehicleAssignementModel request)
        {
            if (request.EmployeeId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.AssignEmployeeToVehicleAsync(request.VehicleId, request.EmployeeId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/unassign-employee")]
        public async Task<ActionResult> UnassignEmployeeFromVehicleAsync(VehicleAssignementModel request)
        {
            if (request.EmployeeId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.UnassignEmployeeFromVehicleAsync(request.EmployeeId, request.VehicleId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/assign-agent")]
        public async Task<ActionResult> AssignAgentToVehicleAsync(AgentAssignmentModel request)
        {
            if (request.AgentId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.AssignAgentToVehicleAsync(request.VehicleId, request.AgentId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/unassign-agent")]
        public async Task<ActionResult> UnassignAgentFromVehicleAsync(AgentAssignmentModel request)
        {
            if (request.AgentId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.UnassignAgentFromVehicleAsync(request.VehicleId, request.AgentId);

            return StatusCode(response.StatusCode, response);
        }
    }
}