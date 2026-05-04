using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;
using BCrypt.Net;

namespace ConstructionSaaS.Api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersAsync(int companyId)
        {
            var users = await _userRepository.GetUsersByCompanyIdAsync(companyId);
            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserResponseDto> CreateUserAsync(int companyId, CreateUserDto dto)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("Email is already registered.");

            var validRoles = new[] { "admin", "staff", "foreman", "viewer" };
            if (!validRoles.Contains(dto.Role.ToLower()))
                throw new Exception($"Invalid role. Valid roles: {string.Join(", ", validRoles)}");

            var user = new User
            {
                CompanyId = companyId,
                Name = dto.Name,
                Email = dto.Email,
                Role = dto.Role.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var userId = await _userRepository.CreateUserAsync(user);

            return new UserResponseDto
            {
                Id = userId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> UpdateUserRoleAsync(int companyId, int userId, UpdateUserRoleDto dto)
        {
            var validRoles = new[] { "admin", "staff", "foreman", "viewer" };
            if (!validRoles.Contains(dto.Role.ToLower()))
                throw new Exception($"Invalid role. Valid roles: {string.Join(", ", validRoles)}");

            return await _userRepository.UpdateUserRoleAsync(companyId, userId, dto.Role.ToLower());
        }

        public async Task<bool> DeleteUserAsync(int companyId, int requestingUserId, int targetUserId)
        {
            // Prevent self-deletion
            if (requestingUserId == targetUserId)
                throw new Exception("You cannot delete your own account.");

            return await _userRepository.DeleteUserAsync(companyId, targetUserId);
        }
    }
}
