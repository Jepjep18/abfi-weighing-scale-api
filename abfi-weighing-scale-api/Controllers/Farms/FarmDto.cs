namespace abfi_weighing_scale_api.Controllers.Farms
{
    public class FarmDto
    {
        public int Id { get; set; }
        public string? FarmName { get; set; }
        public bool IsActive { get; set; }
        //public DateTime CreatedAt { get; set; }
        //public DateTime UpdatedAt { set; get; }
    }

    public class CreateFarmDto
    {
        public string FarmName { get; set; } = string.Empty;
    }

    public class UpdateFarmDto
    {
        public string? FarmName { get; set; }
    }

    public class FileUploadDto
    {
        public IFormFile File { get; set; }
    }
}
