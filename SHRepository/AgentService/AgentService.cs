using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SHDomain.Data;
using SHDomain.Helpers;
using SHDomain.Models;
using SHDomain.Models.Agent;

namespace SHServices.AgentService
{
    public class AgentService(IResponseHelper _responseHelper, AppDbContext _db, ILogger<AgentService> _logger) : IAgentService
    {
        public async Task<ApiResponse<List<Agent>>> GetAllAgentsAsync()
        {
            try
            {
                var agents = await _db.Agents.Where(a => a.IsDeleted == false).ToListAsync();

                return (agents != null)
                    ? _responseHelper.CreateResponse(true, 200, "Agents retrieved successfully.", agents)
                    : _responseHelper.CreateResponse<List<Agent>>(false, 404, "No agents found.", []);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<List<Agent>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<List<Agent>>> GetUnassignedAgentsOnVehiclesAsync()
        {
            try
            {
                var agents = await _db.Agents.Where(a => EF.Functions.Like(a.Type, "vehicle") && a.IsDeleted == false).ToListAsync();

                return _responseHelper.CreateResponse(true, 200, "Agents retrieved successfully.", agents);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<List<Agent>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<List<Agent>>> GetUnassignedAgentsOnApartmentsAsync()
        {
            try
            {
                var agents = await _db.Agents.Where(a => EF.Functions.Like(a.Type, "apartment") && a.IsDeleted == false).ToListAsync();

                return _responseHelper.CreateResponse(true, 200, "Agents retrieved successfully.", agents);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<List<Agent>>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Agent>> SearchAgentAsync(AgentSearchModel searchModel)
        {
            try
            {
                var query = _db.Agents.AsQueryable().Where(c => (c.Id == searchModel.Id || c.Phone == searchModel.Phone || c.Email == searchModel.Email) && c.IsDeleted == false);

                var result = await query.FirstAsync();

                return (result != null)
                    ? _responseHelper.CreateResponse(true, 200, "Agent retrieved successfully.", result)
                    : _responseHelper.CreateResponse<Agent>(false, 404, "Agent not found.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Agent>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Agent>> CreateAgentAsync(Agent agent)
        {
            if (string.IsNullOrWhiteSpace(agent.Phone) ||
                string.IsNullOrWhiteSpace(agent.Email) ||
                string.IsNullOrWhiteSpace(agent.Name) ||
                string.IsNullOrWhiteSpace(agent.Dealer) ||
                string.IsNullOrWhiteSpace(agent.Type) ||
                string.IsNullOrWhiteSpace(agent.Address))
            {
                return _responseHelper.CreateResponse<Agent>(false, 400, "Invalid request. Null object reference", null);
            }

            try
            {
                await _db.Agents.AddAsync(agent);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 201, "Agent created successfully.", agent);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Agent>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<Agent>> UpdateAgentAsync(Agent agent)
        {
            if (string.IsNullOrWhiteSpace(agent.Address) ||
                string.IsNullOrWhiteSpace(agent.Email) ||
                string.IsNullOrWhiteSpace(agent.Name) ||
                string.IsNullOrWhiteSpace(agent.Type))
            {
                return _responseHelper.CreateResponse<Agent>(false, 400, "Invalid request. Required fields are missing.", null);
            }

            try
            {
                var existingAgent = await _db.Agents.FirstOrDefaultAsync(a => a.Id == agent.Id && !a.IsDeleted);
                if (existingAgent == null)
                    return _responseHelper.CreateResponse<Agent>(false, 404, "Agent not found.", null);

                existingAgent.Phone = agent.Phone;
                existingAgent.Dealer = agent.Dealer;
                existingAgent.Email = agent.Email;
                existingAgent.Name = agent.Name;
                existingAgent.Address = agent.Address;
                existingAgent.Type = agent.Type;

                _db.Agents.Update(existingAgent);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse(true, 200, "Agent updated successfully.", existingAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<Agent>(false, 500, ex.Message, null);
            }
        }

        public async Task<ApiResponse<string>> DeleteAgentAsync(int id)
        {
            try
            {
                var agent = await _db.Agents.FindAsync(id);

                if (agent == null)
                    return _responseHelper.CreateResponse<string>(false, 404, "Agent not found.", null);

                agent.IsDeleted = true;

                _db.Agents.Update(agent);
                await _db.SaveChangesAsync();

                return _responseHelper.CreateResponse<string>(true, 200, "Agent deleted successfully.", null);
            }
            catch (Exception ex)
            {
                _logger.LogError(message: ex.Message, ex);
                return _responseHelper.CreateResponse<string>(false, 500, ex.Message, null);
            }
        }
    }
}
