using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int companyId, int id);
        Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId);
        Task<bool> UpdateUserRoleAsync(int companyId, int id, string role);
        Task<bool> DeleteUserAsync(int companyId, int id);
    }
}
