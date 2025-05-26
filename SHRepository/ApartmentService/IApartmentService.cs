using SHDomain.Models;
using SHDomain.Models.Apartment;

namespace SHServices.ApartmentService
{
    public interface IApartmentService
    {
        /** Apartment CRUD Operations **/
        Task<ApiResponse<IEnumerable<Apartment>>> GetAllApartmentsAsync();
        Task<ApiResponse<Apartment>> GetApartmentBySearchAsync(ApartmentSearchModel searchModel);
        Task<ApiResponse<Apartment>> CreateApartmentAsync(Apartment apartment);
        Task<ApiResponse<Apartment>> UpdateApartmentAsync(Apartment apartment);
        Task<ApiResponse<bool>> DeleteApartmentByIdAsync(int apartmentId);

        /** Employee-Apartment Assignment **/
        Task<ApiResponse<Apartment>> AssignEmployeeToApartmentAsync(int apartmentId, int employeeId);
        Task<ApiResponse<Apartment>> UnassignEmployeeFromApartmentAsync(int apartmentId, int employeeId);

        /** Agent-Apartment Assignment **/
        Task<ApiResponse<Apartment>> AssignAgentToApartmentAsync(int apartmentId, int agentId);
        Task<ApiResponse<Apartment>> UnassignAgentFromApartmentAsync(int apartmentId, int agentId);
    }
}
