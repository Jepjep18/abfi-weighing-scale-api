using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.ProductClassification
{
    public class ProductClassificationDto
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty;
        public string IndividualWeightRange { get; set; } = string.Empty;
        public string TotalWeightRangePerCrate { get; set; } = string.Empty;
        public int NoOfHeadsPerGalantina { get; set; }
        public string CratesWeight { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }

    public class CreateProductClassificationDto
    {
        public string ProductCode { get; set; } = string.Empty;
        public string IndividualWeightRange { get; set; } = string.Empty;
        public string TotalWeightRangePerCrate { get; set; } = string.Empty;
        public int? NoOfHeadsPerGalantina { get; set; }
        public string CratesWeight { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
    }

    public class UpdateProductClassificationDto
    {
        public string? ProductCode { get; set; }
        public string? IndividualWeightRange { get; set; }
        public string? TotalWeightRangePerCrate { get; set; }
        public int? NoOfHeadsPerGalantina { get; set; }
        public string? CratesWeight { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ProductClassificationListDto
    {
        public int Id { get; set; }
        public string? ProductCode { get; set; }
    }

    // File Upload DTO (you already have this)
    public class ProductClassificationFileUploadDto
    {
        public IFormFile File { get; set; } = default!;
    }
}
