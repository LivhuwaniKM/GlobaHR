using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SHDomain.Helpers;
using SHDomain.Models.Agent;
using SHDomain.Models.Apartment;
using SHServices.AgentService;

namespace StaffHR.Controllers
{
    public class AgentsController(IAgentService _agentService, IResponseHelper _responseHelper) : BaseController
    {
        [HttpGet("/api/agent/all")]
        public async Task<ActionResult> GetAllAgentsAsync()
        {
            var response = await _agentService.GetAllAgentsAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/agent/unassigned/vehicles/list")]
        public async Task<ActionResult> GetAllUnassignedAgentsOnVehiclesAsync()
        {
            var response = await _agentService.GetUnassignedAgentsOnVehiclesAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/agent/unassigned/apartments/list")]
        public async Task<ActionResult> GetAllUnassignedAgentsOnApartmentsAsync()
        {
            var response = await _agentService.GetUnassignedAgentsOnApartmentsAsync();

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("/api/agent/search")]
        public async Task<ActionResult> SearchAgentAsync([FromQuery] AgentSearchModel searchModel)
        {
            var response = await _agentService.SearchAgentAsync(searchModel);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("/api/agent/create")]
        public async Task<ActionResult> CreateAgentAsync(Agent agent)
        {
            var response = await _agentService.CreateAgentAsync(agent);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("/api/agent/update/{id}")]
        public async Task<ActionResult> UpdateAgentAsync(int id, [FromBody] Agent agent)
        {
            if (agent.Id != id || agent == null)
            {
                var err = _responseHelper.CreateResponse<Apartment>(false, 400, "Invalid request. Null object reference", null);
                return StatusCode(err.StatusCode, err);
            }
            var response = await _agentService.UpdateAgentAsync(agent);

            return StatusCode(response.StatusCode, response);
        }

        [Authorize(Roles = "SuperAdministrator")]
        [HttpPost("/api/agent/delete/{id}")]
        public async Task<ActionResult> DeleteAgentAsync(int id)
        {
            var response = await _agentService.DeleteAgentAsync(id);

            return StatusCode(response.StatusCode, response);
        }
    }
}