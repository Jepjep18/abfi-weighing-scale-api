//using abfi_weighing_scale_api.Data;
//using abfi_weighing_scale_api.Models.Entities;
//using abfi_weighing_scale_api.Repositories.Interfaces;
//using Microsoft.EntityFrameworkCore;

//namespace abfi_weighing_scale_api.Repositories.Implementations
//{
//    public class UserRepository : IUserRepository
//    {
//        private readonly AppDbContext _context;

//        public UserRepository(AppDbContext context)
//        {
//            _context = context;
//        }

//        public Task<User?> GetByIdAsync(int id) =>
//            _context.Users.FindAsync(id).AsTask();

//        public Task<User?> GetByEmailAsync(string email) =>
//            _context.Users.FirstOrDefaultAsync(x => x.Email == email);

//        public Task AddAsync(User user) =>
//            _context.Users.AddAsync(user).AsTask();

//        public Task SaveChangesAsync() =>
//            _context.SaveChangesAsync();
//    }
//}
