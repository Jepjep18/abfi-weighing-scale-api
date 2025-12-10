namespace abfi_weighing_scale_api.Models.Entities
{
    public class Farms
    {
        public int Id { get; set; }
        public string? FarmName { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
