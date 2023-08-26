using Microsoft.AspNetCore.Http;
using ProductCatalog.Service.IRepository;
using System.Security.Claims;

namespace ProductCatalog.Service.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(IHttpContextAccessor httpContextAccessor)
        {
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
    }
}