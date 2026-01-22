//using abfi_weighing_scale_api.Models.Dtos;
//using abfi_weighing_scale_api.Models.Entities;
//using abfi_weighing_scale_api.Repositories.Interfaces;
//using abfi_weighing_scale_api.Services.Interfaces;

//namespace abfi_weighing_scale_api.Services.Implementations
//{
//    public class UserService : IUserService
//    {
//        private readonly IUserRepository _repo;

//        public UserService(IUserRepository repo)
//        {
//            _repo = repo;
//        }

//        public async Task<UserResponseDto> CreateUserAsync(UserCreateDto dto)
//        {
//            var user = new User
//            {
//                Email = dto.Email,
//                FullName = dto.FullName,
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
//                CreatedAt = DateTime.UtcNow
//            };

//            await _repo.AddAsync(user);
//            await _repo.SaveChangesAsync();

//            return new UserResponseDto
//            {
//                Id = user.Id,
//                Email = user.Email,
//                FullName = user.FullName
//            };
//        }

//        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
//        {
//            var user = await _repo.GetByEmailAsync(email);
//            if (user == null) return null;

//            return new UserResponseDto
//            {
//                Id = user.Id,
//                Email = user.Email,
//                FullName = user.FullName
//            };
//        }
//    }

//}
