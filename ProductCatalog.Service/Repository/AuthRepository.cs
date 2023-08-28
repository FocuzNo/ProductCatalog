using Microsoft.Extensions.Configuration;
using ProductCatalog.DAL.Entities;
using ProductCatalog.DAL;
using ProductCatalog.Service.IRepository;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace ProductCatalog.Service.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        private static User user = new();

        public AuthRepository(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _configuration = configuration;
        }

        public async Task RegisterAccount(UserDto userDto)
        {
            string passworHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            user.Username = userDto.Username;
            user.PasswordHash = passworHash;
            user.Role = userDto.Role;
            await _dataContext.AddAsync(user);
            _dataContext.SaveChanges();
        }

        public string GenerateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWT:Token").Value!));

            var signCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.Now.AddHours(1),
               signingCredentials: signCred
           );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return jwtToken;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(3)
            };

            return refreshToken;
        }
    }
}