namespace abfi_weighing_scale_api.Controllers.Production
{
    //public class ProductionDto
    //{
    //    public int Id { get; set; }
    //    public int FarmId { get; set; }
    //    public string? FarmName { get; set; }
    //    public int? ForcastedTrips { get; set; }
    //    public int? NoOfHeads { get; set; }
    //    public DateTime? StartDateTime { get; set; }
    //    public DateTime? EndDateTime { get; set; }
    //    public DateTime? CreatedAt { get; set; }
    //}

    public class ProductionWithFarmDto
    {
        public int Id { get; set; }
        public int FarmId { get; set; }
        public string? FarmName { get; set; }
        public int? ForcastedTrips { get; set; }
        public int? NoOfHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public FarmInfoDto? Farm { get; set; }
    }

    public class FarmInfoDto
    {
        public int Id { get; set; }
        public string? FarmName { get; set; }
        public bool IsActive { get; set; }
    }

    // Remove EndDateTime from CreateProductionDto
    public class CreateProductionDto
    {
        public string? ProductionName { get; set; }
        public int? TotalHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        //public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
        public List<ProductionFarmCreateDto> FarmDetails { get; set; } = new();
    }

    public class ProductionFarmCreateDto
    {
        public int FarmId { get; set; }
        public int? ForecastedTrips { get; set; }
        public int? AllocatedHeads { get; set; }
    }

    public class ProductionDto
    {
        public int Id { get; set; }
        public string? ProductionName { get; set; }
        public int? TotalHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Description { get; set; }
        public List<ProductionFarmDetailDto> FarmDetails { get; set; } = new();
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

    public class UpdateProductionDto
    {
        public string? ProductionName { get; set; }
        public int? TotalHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateProductionFarmsDto
    {
        public List<ProductionFarmCreateDto> FarmDetails { get; set; } = new();
    }

    public class ProductionListDto
    {
        public int Id { get; set; }
        public string? ProductionName { get; set; }
        public int? TotalHeads { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int FarmCount { get; set; }
        //public List<string> FarmNames { get; set; } = new();
        public List<ProductionFarmDetailDto> FarmDetails { get; set; } = new(); // Add this line

    }

    // ProductionRequestDto.cs
    public class ProductionRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
    }

    
}
