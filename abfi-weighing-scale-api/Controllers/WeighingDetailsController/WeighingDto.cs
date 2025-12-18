namespace abfi_weighing_scale_api.Controllers.WeighingDetailsController
{
    public class WeighingDto
    {
    }

    public class WeighingRequestDto
    {
        public string SerialData { get; set; }
        public string PortNumber { get; set; }
    }

    public class WeighingDetailDto
    {
        public int Id { get; set; }
        public string SerialData { get; set; }
        public decimal? Qty { get; set; }
        public string UoM { get; set; }
        public int? Heads { get; set; }
        public string ProdCode { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string PortNumber { get; set; }
        public string Class { get; set; }
        public string Remarks { get; set; }
    }

    public class ProcessedWeighingDataDto
    {
        public decimal Qty { get; set; }
        public string UoM { get; set; }
        public int? NumHeads { get; set; }
        public string ProdCode { get; set; }
        public string Class { get; set; }
        public string Remarks { get; set; }
    }

    public class DateRangeRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
