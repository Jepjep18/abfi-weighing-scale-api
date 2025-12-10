namespace abfi_weighing_scale_api.Models.Entities
{
    public class ProductClassification
    {
        public int Id { get; set; }
        public string? ProductCode { get; set; }
        public string? IndividualWeightRange { get; set; }
        public string? TotalWeightRangePerCrate { get; set; }
        public int? NoOfHeadsPerGalantina { get; set; }
        public string? CratesWeight { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
    }
}
