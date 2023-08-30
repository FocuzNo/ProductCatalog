using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            if(string.IsNullOrWhiteSpace(newUser.Username) || string.IsNullOrWhiteSpace(newUser.PasswordHash))
            {
                return BadRequest("Invalid username or password. Check the fields. Fields cannot be empty.");
            }

            return Ok(newUser);
        }

        [HttpDelete("DeleteUser/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(int? id)
        {
            await _userRepository.DeleteUser(id);
            return Ok();
        }

        [HttpPut("EditPassUser"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> EditPassword(string? name, string? newPassword)
        {
            await _userRepository.EditPassword(name, newPassword);
            return Ok(newPassword);
        }

        [HttpPut("BlockedUser/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> Blocked(int? id)
        {
            await _userRepository.BlockedUser(id);
            return Ok();
        }

        [HttpGet("GetUsers"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetProducts()
        {
            var product = await _userRepository.GetUsers();
            return Ok(product);
        }

        [HttpGet("GetUserById/{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            return Ok(user);
        }
    }
}