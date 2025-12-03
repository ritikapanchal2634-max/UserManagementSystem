using UserManagementSystem.Models.Entities;
using UserManagementSystem.Models.ViewModels;

namespace UserManagementSystem.IServices
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterViewModel model, List<string> documentPaths);
        Task<User> AuthenticateAsync(string username, string password);
        Task<bool> UserExistsAsync(string username);
        Task<PaginatedUserListViewModel> GetUsersAsync(int page, int pageSize, string sortBy, string sortOrder, string searchTerm);
        Task<User> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(EditUserViewModel model, List<string> newDocumentPaths);
        Task<bool> DeleteUserAsync(int id);
        Task<List<State>> GetStatesAsync();
        Task<List<City>> GetCitiesByStateAsync(int stateId);
    }
}
