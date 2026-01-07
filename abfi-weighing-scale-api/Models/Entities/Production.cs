namespace abfi_weighing_scale_api.Models.Entities
{
    public class Production
    {
        public int Id { get; set; }
        public string? ProductionName { get; set; } // Optional: give a name to the production
        public int? TotalHeads { get; set; } // Total heads across all farms
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; } // Optional: description

        // Navigation properties
        public virtual ICollection<ProductionFarm> ProductionFarms { get; set; } = new List<ProductionFarm>();
        public virtual ICollection<WeighingDetail> WeighingDetails { get; set; }
        public virtual ICollection<WeighingProductionGroup> WeighingProductionGroups { get; set; } = new List<WeighingProductionGroup>();
    }
}
