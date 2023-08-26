using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IAuthRepository
    {
        void RegisterAccount(UserDto userDto);
        string GenerateToken(User user);
        //RefreshToken GenerateRefreshToken();
    }
}
