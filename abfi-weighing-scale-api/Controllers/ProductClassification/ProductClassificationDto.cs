using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.ProductClassification
{
    public class ProductClassificationDto
    {
    }

    public class ProductClassificationFileUploadDto
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; } = default!;
    }

    public class ProductClassificationCsvRow
    {
        public string ProductCode { get; set; } = "";
        public string IndividualWeightRange { get; set; } = "";
        public string TotalWeightRangePerCrate { get; set; } = "";
        public int NoOfHeadsPerGalantina { get; set; }
        public decimal CratesWeight { get; set; }
    }

    public class ProductClassificationListDto
    {
        public int Id { get; set; }
        public string? ProductCode { get; set; }
    }
}
