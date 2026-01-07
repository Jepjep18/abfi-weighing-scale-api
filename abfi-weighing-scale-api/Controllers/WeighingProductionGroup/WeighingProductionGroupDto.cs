using System.ComponentModel.DataAnnotations;

namespace abfi_weighing_scale_api.Models.DTOs.WeighingProductionGroup
{
    public class CreateWeighingProductionGroupDto
    {
        [Required]
        public int ProductionId { get; set; }
    }

    public class WeighingProductionGroupResponseDto
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ProductionInfoDto? Production { get; set; }
    }

    public class ProductionInfoDto
    {
        public int Id { get; set; }
        public string? ProductionName { get; set; }
        public int? TotalHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public List<ProductionFarmDetailDto> FarmDetails { get; set; } = new List<ProductionFarmDetailDto>();
    }

    public class ProductionFarmDetailDto
    {
        public int Id { get; set; }
        public int ProductionId { get; set; }
        public int FarmId { get; set; }
        public string? FarmName { get; set; }
        public int? ForecastedTrips { get; set; }
        public int? AllocatedHeads { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}