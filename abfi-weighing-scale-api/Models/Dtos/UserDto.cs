namespace abfi_weighing_scale_api.Models.Dtos
{
    public class UserDto
    {
    }

    public class UserCreateDto
    {
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class UserUpdateDto
    {
        public string FullName { get; set; } = default!;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
    }



}
