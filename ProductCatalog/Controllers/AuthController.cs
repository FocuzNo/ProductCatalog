using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DAL;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IAuthRepository _authRepository;

        private readonly static User user = new();

        public AuthController(DataContext dataContext, IAuthRepository authRepository)
        {
            _dataContext = dataContext;
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public ActionResult Register(UserDto userDto)
        {
            var users = _dataContext.Users.ToList();
            foreach (User user in users)
            {
                if (user.Username == userDto.Username)
                {
                    return BadRequest("Such user already exists. " +
                        "Try again.");
                }
            }

            _authRepository.RegisterAccount(userDto);
            return Ok(userDto);
        }

        [HttpPost("Login")]
        public ActionResult Login(UserDto userDto)
        {
            User? user =
                 _dataContext.Users.FirstOrDefault(u => u.Username == userDto.Username);

            if (user == null)
            {
                return BadRequest("Wrong username.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            var token = _authRepository.GenerateToken(user);

            //var refreshToken = _authRepository.GenerateRefreshToken();
            //SetRefreshToken(refreshToken);

            return Ok(user);
        }
    }
}
