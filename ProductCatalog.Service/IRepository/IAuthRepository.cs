using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IAuthRepository
    {
        Task RegisterAccount(UserDto userDto);
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken();
    }
}