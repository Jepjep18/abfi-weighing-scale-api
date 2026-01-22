namespace abfi_weighing_scale_api.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? Lastname { get; set; }
        public string? BusinessUnit { get; set; }
        public string? Department { get; set; }
        public string? Role { get; set; }

        public bool? IsActive { get; set; }
        public string PasswordHash { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }

}
