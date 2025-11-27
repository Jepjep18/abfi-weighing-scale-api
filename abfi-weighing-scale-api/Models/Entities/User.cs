namespace abfi_weighing_scale_api.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

}
