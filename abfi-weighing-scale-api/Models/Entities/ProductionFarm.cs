namespace abfi_weighing_scale_api.Models.Entities
{
    public class ProductionFarm
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public int FarmId { get; set; }
        public int? ForecastedTrips { get; set; } // Farm-specific forecasted trips
        public int? AllocatedHeads { get; set; } // Heads allocated to this specific farm
        public DateTime? CreatedAt { get; set; }

        // Navigation properties
        public virtual Production? Production { get; set; }
        public virtual Farms? Farm { get; set; }
    }
}
