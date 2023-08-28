using Microsoft.AspNetCore.Http;
using ProductCatalog.DAL.Entities;
using ProductCatalog.DAL;
using ProductCatalog.Service.IRepository;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.EntityFrameworkCore;

namespace ProductCatalog.Service.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetName()
        {
            var name = string.Empty;

            if (_httpContextAccessor.HttpContext is not null)
            {
                name = _httpContextAccessor.HttpContext.User?
                    .FindFirstValue(ClaimTypes.Name);
            }

            return name!;
        }

        public async Task AddUser(User user)
        {
            _dataContext.Add(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteUser(string? name)
        {
            User? user = _dataContext.Users.FirstOrDefault(x => x.Username == name);
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
        }

        public async Task EditPassword(string? name, string? newPassword)
        {
            User? user = _dataContext.Users.FirstOrDefault(x => x.Username == name);
            if (user is not null)
            {
                string passworHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.PasswordHash = passworHash;
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task BlockedUser(int? id)
        {
            var user = _dataContext.Users.FirstOrDefault(i => i.Id == id);
            if (user != null)
            {
                user.Blocked = true;
                _dataContext.Users.Update(user);
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}