using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DAL;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetName"), Authorize (Roles= "Admin")]
        public ActionResult<string> GetName()
        {
            return Ok(_userRepository.GetName());
        }

        [HttpPost("AddUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddUser(User newUser)
        {
            await _userRepository.AddUser(newUser);
            return Ok(newUser);
        }

        [HttpDelete("DeleteUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(string? name)
        {
            if (name is null)
            {
                return BadRequest("No such user found. " +
                    "Try again.");
            }

            await _userRepository.DeleteUser(name);
            return Ok();
        }

        [HttpPut("EditPassUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditPassword(string? name, string? newPassword)
        {
            await _userRepository.EditPassword(name, newPassword);
            return Ok(newPassword);
        }

        [HttpPost("BlockedUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Blocked(int? id)
        {
            await _userRepository.BlockedUser(id);
            return Ok();
        }
    }
}