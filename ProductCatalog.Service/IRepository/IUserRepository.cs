using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IUserRepository
    {
        string GetName();
        Task AddUser(User user);
        Task DeleteUser(int? id);
        Task EditPassword(string? name, string? password);
        Task BlockedUser(int? id);
        Task<List<User>> GetUsers();
        Task<User?> GetUserById(int? id);
    }
}