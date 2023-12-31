﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.DAL;
using ProductCatalog.DAL.Entities;
using ProductCatalog.Service.IRepository;
using System.Security.Claims;

namespace ProductCatalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IAuthRepository _authRepository;

        private readonly static User user = new();

        public AuthController(DataContext dataContext, IAuthRepository authRepository, IUserRepository userRepository)
        {
            _dataContext = dataContext;
            _authRepository = authRepository;
        }

        [HttpPost("Register")]
        public async Task <ActionResult> Register(UserDto userDto)
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

            if (string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest("Incorrect data. Put fields or incorrect username or password.");
            }
            await _authRepository.RegisterAccount(userDto);

            return Ok(userDto);
        }

        [HttpPost("Login")]
        public ActionResult Login(UserDto userDto)
        {
            User? user =
                 _dataContext.Users.FirstOrDefault(u => u.Username == userDto.Username);
            if(user?.Blocked is true)
            {
                return BadRequest("You are blocked.");
            }

            if (user is null)
            {
                return BadRequest("Wrong username.");
            }

            if (!BCrypt.Net.BCrypt.Verify(userDto.Password, user.PasswordHash))
            {
                return BadRequest("Wrong password.");
            }

            var token = _authRepository.GenerateToken(user);

            var refreshToken = _authRepository.GenerateRefreshToken();
            SetRefreshToken(refreshToken);

            return Ok(token);
        }

        [HttpPost("RefreshToken"), Authorize (Roles = "Admin, SuperUser, User")]
        public ActionResult<string> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (user.RefreshToken.Equals(refreshToken))
            {
                return Unauthorized("Invalid refresh token.");
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            var token = _authRepository.GenerateToken(user);

            var newRefreshToken = _authRepository.GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);
            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.CreateToken;
            user.TokenExpires = newRefreshToken.Expires;
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public ActionResult<object> GetRoles()
        {
            var userName = User?.Identity?.Name;
            var userName2 = User?.FindFirstValue(ClaimTypes.Name);
            var role = User?.FindFirstValue(ClaimTypes.Role);

            return Ok(new { userName, userName2, role });
        }
    }
}