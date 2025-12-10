namespace abfi_weighing_scale_api.Models.Entities
{
    public class Production
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public int? ForcastedTrips { get; set; }
        public int? NoOfHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public virtual Farms? Farm { get; set; }

    }
}
