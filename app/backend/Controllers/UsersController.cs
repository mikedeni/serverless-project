using ConstructionSaaS.Api.DTOs;
using ConstructionSaaS.Api.Extensions;
using ConstructionSaaS.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Api.Controllers
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            var users = await _userService.GetUsersAsync(companyId);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var newUser = await _userService.CreateUserAsync(companyId, dto);
                return CreatedAtAction(nameof(GetUsers), null, newUser);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] UpdateUserRoleDto dto)
        {
            var companyId = User.GetCompanyId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var success = await _userService.UpdateUserRoleAsync(companyId, id, dto);
                if (!success) return NotFound("User not found or unauthorized.");

                return Ok(new { Message = "User role updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var companyId = User.GetCompanyId();
            var userId = User.GetUserId();
            if (companyId == 0) return Unauthorized("Invalid Company Context");

            try
            {
                var success = await _userService.DeleteUserAsync(companyId, userId, id);
                if (!success) return NotFound("User not found or unauthorized.");

                return Ok(new { Message = "User removed successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
