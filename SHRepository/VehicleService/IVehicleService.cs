using SHDomain.Models;
using SHDomain.Models.Vehicle;

namespace SHServices.VehicleService
{
    public interface IVehicleService
    {
        Task<ApiResponse<IEnumerable<Vehicle>>> GetAllVehiclesAsync();
        Task<ApiResponse<Vehicle>> CreateVehicleAsync(Vehicle Vehicle);
        Task<ApiResponse<Vehicle>> UpdateVehicleAsync(Vehicle Vehicle);
        Task<ApiResponse<Vehicle>> SearchVehicleAsync(VehicleSearchModel searchModel);
        Task<ApiResponse<string>> DeleteVehicleAsync(string vin);
        Task<ApiResponse<Vehicle>> AssignVehicleAsync(int? employeeId, int? vehicleId);
        Task<ApiResponse<Vehicle>> UnassignVehicleAsync(int? employeeId, int? vehicleId);
        Task<ApiResponse<Vehicle>> AssignAgentOnVehicleAsync(int? employeeId, int? agentId);
        Task<ApiResponse<Vehicle>> UnassignAgentVehicleAsync(int? employeeId, int? agentId);
    }
}
