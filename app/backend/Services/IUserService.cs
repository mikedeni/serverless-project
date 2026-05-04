using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;

namespace ConstructionSaaS.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetUsersAsync(int companyId);
        Task<UserResponseDto> CreateUserAsync(int companyId, CreateUserDto dto);
        Task<bool> UpdateUserRoleAsync(int companyId, int userId, UpdateUserRoleDto dto);
        Task<bool> DeleteUserAsync(int companyId, int requestingUserId, int targetUserId);
    }
}
