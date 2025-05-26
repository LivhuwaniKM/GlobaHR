using SHDomain.Models;
using SHDomain.Models.Vehicle;

namespace SHServices.VehicleService
{
    public interface IVehicleService
    {
        /** Vehicle CRUD Operations **/
        Task<ApiResponse<IEnumerable<Vehicle>>> GetAllVehiclesAsync();
        Task<ApiResponse<Vehicle>> CreateVehicleAsync(Vehicle Vehicle);
        Task<ApiResponse<Vehicle>> UpdateVehicleAsync(Vehicle Vehicle);
        Task<ApiResponse<Vehicle>> SearchVehicleAsync(VehicleSearchModel searchModel);
        Task<ApiResponse<bool>> DeleteVehicleAsync(string vin);

        /** Employee-Vehicle Assignment **/
        Task<ApiResponse<Vehicle>> AssignEmployeeToVehicleAsync(int vehicleId, int employeeId);
        Task<ApiResponse<Vehicle>> UnassignEmployeeFromVehicleAsync(int vehicleId, int employeeId);

        /** Agent-Apartment Assignment **/
        Task<ApiResponse<Vehicle>> AssignAgentToVehicleAsync(int vehicleId, int agentId);
        Task<ApiResponse<Vehicle>> UnassignAgentFromVehicleAsync(int vehicleId, int agentId);
    }
}
