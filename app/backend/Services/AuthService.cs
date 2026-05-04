using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Models;
using ConstructionSaaS.Api.Repositories;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace ConstructionSaaS.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepository, ICompanyRepository companyRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;
            _config = config;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
                throw new Exception("Email is already registered.");

            var companyId = await _companyRepository.CreateCompanyAsync(request.CompanyName);

            var user = new User
            {
                CompanyId = companyId,
                Name = request.Name,
                Email = request.Email,
                Role = "admin", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            var userId = await _userRepository.CreateUserAsync(user);

            var token = GenerateJwtToken(userId, companyId, user.Role);

            return new AuthResponse
            {
                Token = token,
                UserId = userId,
                CompanyId = companyId,
                Role = user.Role
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid email or password.");

            var token = GenerateJwtToken(user.Id, user.CompanyId, user.Role);

            return new AuthResponse
            {
                Token = token,
                UserId = user.Id,
                CompanyId = user.CompanyId,
                Role = user.Role
            };
        }

        private string GenerateJwtToken(int userId, int companyId, string role)
        {
            var jwtKey = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim("user_id", userId.ToString()),
                new Claim("company_id", companyId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
