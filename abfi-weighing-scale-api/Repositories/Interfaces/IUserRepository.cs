using abfi_weighing_scale_api.Models.Entities;

namespace abfi_weighing_scale_api.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }

}
