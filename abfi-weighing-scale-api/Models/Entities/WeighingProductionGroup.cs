namespace abfi_weighing_scale_api.Models.Entities
{
    public class WeighingProductionGroup
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Production Production { get; set; } = null!;

    }
}
