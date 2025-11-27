using abfi_weighing_scale_api.Models.Dtos;

namespace abfi_weighing_scale_api.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUserAsync(UserCreateDto dto);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);

    }

}
