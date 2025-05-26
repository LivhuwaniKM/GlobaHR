using SHDomain.Models;
using SHDomain.Models.Agent;

namespace SHServices.AgentService
{
    public interface IAgentService
    {
        Task<ApiResponse<List<Agent>>> GetAllAgentsAsync();
        Task<ApiResponse<List<Agent>>> GetUnassignedAgentsOnVehiclesAsync();
        Task<ApiResponse<List<Agent>>> GetUnassignedAgentsOnApartmentsAsync();
        Task<ApiResponse<Agent>> SearchAgentAsync(AgentSearchModel searchModel);
        Task<ApiResponse<Agent>> CreateAgentAsync(Agent agent);
        Task<ApiResponse<Agent>> UpdateAgentAsync(Agent agent);
        Task<ApiResponse<string>> DeleteAgentAsync(int id);
    }
}
