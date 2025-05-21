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
        [HttpGet("/api/vehicle/get/all")]
        public async Task<ActionResult> GetAllVehicles()
        {
            var response = await _vehicleService.GetAllVehiclesAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/vehicle/update/{vehicleId}")]
        public async Task<ActionResult> UpdateVehicleAsync(int vehicleId, [FromBody] Vehicle vehicle)
        {
            if (vehicleId <= 0 || vehicle == null || vehicleId != vehicle.Id)
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
        [HttpPost("/api/vehicle/delete/{vin}")]
        public async Task<ActionResult> DeleteVehicleAsync(string vin)
        {
            var response = await _vehicleService.DeleteVehicleAsync(vin);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/assign")]
        public async Task<ActionResult> AssignVehicleAsync(VehicleAssignementModel request)
        {
            if (request.EmployeeId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.AssignVehicleAsync(request.EmployeeId, request.VehicleId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/vehicle/unassign")]
        public async Task<ActionResult> UnassignVehicleAsync(VehicleAssignementModel request)
        {
            if (request.EmployeeId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.UnassignVehicleAsync(request.EmployeeId, request.VehicleId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/agent/assign")]
        public async Task<ActionResult> AssignAgentOnVehicleAsync(AgentAssignmentModel request)
        {
            if (request.AgentId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.AssignVehicleAsync(request.AgentId, request.VehicleId);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/agent/unassign")]
        public async Task<ActionResult> UnassignAgentOnVehicleAsync(AgentAssignmentModel request)
        {
            if (request.AgentId < 0 || request.VehicleId < 0)
            {
                var error = _responseHelper.CreateResponse<Vehicle>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(error.StatusCode, error);
            }
            var response = await _vehicleService.UnassignVehicleAsync(request.AgentId, request.VehicleId);

            return StatusCode(response.StatusCode, response);
        }
    }
}