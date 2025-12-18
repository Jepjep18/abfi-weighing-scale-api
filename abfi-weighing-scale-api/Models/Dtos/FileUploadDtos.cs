namespace abfi_weighing_scale_api.Models.Dtos
{
    public class FileUploadDtos
    {
    }

    public class WeighingDetailsFileUploadDto
    {
        public IFormFile File { get; set; }
    }

    public class ProdClassificationFileUploadDto
    {
        public IFormFile File { get; set; }
    }

    public class PortClassificationFileUploadDto
    {
        public IFormFile File { get; set; }
    }

    public class UploadResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
