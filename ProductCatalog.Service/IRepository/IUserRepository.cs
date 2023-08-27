using ProductCatalog.DAL.Entities;

namespace ProductCatalog.Service.IRepository
{
    public interface IUserRepository
    {
        string GetName();
        Task AddUser(User user);
        Task DeleteUser(string? name);
        Task EditPassword(string? name, string? password);
        Task BlockedUser(int? id);
    }
}