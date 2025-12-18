using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Services.WeighingService;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.WeighingDetailsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeighingController : ControllerBase
    {
        private readonly IWeighingService _weighingService;

        public WeighingController(IWeighingService weighingService)
        {
            _weighingService = weighingService;
        }

        /// <summary>
        /// Save weighing data from serial port
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SaveWeighingData([FromBody] WeighingRequestDto request)
        {
            try
            {
                var result = await _weighingService.SaveWeighingDataAsync(request);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get all weighing data
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllWeighingData()
        {
            try
            {
                var data = await _weighingService.GetAllWeighingDataAsync();
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get weighing data by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWeighingDataById(int id)
        {
            try
            {
                var data = await _weighingService.GetWeighingDataByIdAsync(id);

                if (data == null)
                    return NotFound(new { success = false, error = "Weighing data not found" });

                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Get weighing data by date range
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetWeighingDataByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var data = await _weighingService.GetWeighingDataByDateRangeAsync(startDate, endDate);
                return Ok(new { success = true, data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint to simulate receiving data from weighing scale
        /// </summary>
        [HttpPost("test")]
        public async Task<IActionResult> TestWeighingData()
        {
            try
            {
                // Test data similar to what the Windows Service sends
                var testRequest = new WeighingRequestDto
                {
                    SerialData = "16.3 KG G 000000 PCS",
                    PortNumber = "COM11"
                };

                var result = await _weighingService.SaveWeighingDataAsync(testRequest);
                return Ok(new
                {
                    success = true,
                    message = "Test data processed successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // ===================== UPLOAD ENDPOINTS =====================

        /// <summary>
        /// Upload WeighingDetails data from Excel file
        /// </summary>
        [HttpPost("upload/weighing-details")]
        public async Task<IActionResult> UploadWeighingDetails([FromForm] WeighingDetailsFileUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest(new { success = false, message = "No file uploaded." });

            var result = await _weighingService.UploadWeighingDetailsAsync(dto.File);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });

            return Ok(new
            {
                success = true,
                message = result.Message,
                recordsInserted = result.Count,
                errors = result.Errors
            });
        }

        /// <summary>
        /// Upload ProdClassification data from Excel file
        /// </summary>
        [HttpPost("upload/prod-classification")]
        public async Task<IActionResult> UploadProdClassification([FromForm] ProdClassificationFileUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest(new { success = false, message = "No file uploaded." });

            var result = await _weighingService.UploadProdClassificationAsync(dto.File);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });

            return Ok(new
            {
                success = true,
                message = result.Message,
                recordsInserted = result.Count,
                errors = result.Errors
            });
        }

        /// <summary>
        /// Upload PortClassification data from Excel file
        /// </summary>
        [HttpPost("upload/port-classification")]
        public async Task<IActionResult> UploadPortClassification([FromForm] PortClassificationFileUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest(new { success = false, message = "No file uploaded." });

            var result = await _weighingService.UploadPortClassificationAsync(dto.File);

            if (!result.Success)
                return BadRequest(new { success = false, message = result.Message, errors = result.Errors });

            return Ok(new
            {
                success = true,
                message = result.Message,
                recordsInserted = result.Count,
                errors = result.Errors
            });
        }

        // ===================== DOWNLOAD TEMPLATE ENDPOINTS =====================

        /// <summary>
        /// Download Excel template for WeighingDetails
        /// </summary>
        [HttpGet("download-template/weighing-details")]
        public IActionResult DownloadWeighingDetailsTemplate()
        {
            try
            {
                var stream = new MemoryStream();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("WeighingDetails");

                    // Add headers
                    worksheet.Cell(1, 1).Value = "SerialData";
                    worksheet.Cell(1, 2).Value = "Qty";
                    worksheet.Cell(1, 3).Value = "UoM";
                    worksheet.Cell(1, 4).Value = "Heads";
                    worksheet.Cell(1, 5).Value = "ProdCode";
                    worksheet.Cell(1, 6).Value = "CreatedDateTime";
                    worksheet.Cell(1, 7).Value = "PortNumber";
                    worksheet.Cell(1, 8).Value = "Class";
                    worksheet.Cell(1, 9).Value = "Remarks";

                    // Add sample data
                    worksheet.Cell(2, 1).Value = "16.3 KG G 000000 PCS";
                    worksheet.Cell(2, 2).Value = 16.300;
                    worksheet.Cell(2, 3).Value = "KG";
                    worksheet.Cell(2, 4).Value = 15;
                    worksheet.Cell(2, 5).Value = "CBL";
                    worksheet.Cell(2, 6).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    worksheet.Cell(2, 7).Value = "COM11";
                    worksheet.Cell(2, 8).Value = "ClassB";
                    worksheet.Cell(2, 9).Value = "NULL";

                    // Format header
                    var headerRange = worksheet.Range("A1:I1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Auto-fit columns
                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(stream);
                }

                stream.Position = 0;

                return File(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "WeighingDetails_Template.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = $"Error creating template: {ex.Message}" });
            }
        }

        /// <summary>
        /// Download Excel template for ProdClassification
        /// </summary>
        [HttpGet("download-template/prod-classification")]
        public IActionResult DownloadProdClassificationTemplate()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("ProdClassification");

                // Add headers
                worksheet.Cell(1, 1).Value = "ProdCode";
                worksheet.Cell(1, 2).Value = "IndvWeight_Min";
                worksheet.Cell(1, 3).Value = "IndvWeight_Max";
                worksheet.Cell(1, 4).Value = "TotalIndvWeight_Min";
                worksheet.Cell(1, 5).Value = "TotalIndvWeight_Max";
                worksheet.Cell(1, 6).Value = "CratesWeight_Min";
                worksheet.Cell(1, 7).Value = "CratesWeight_Max";
                worksheet.Cell(1, 8).Value = "NumHeads";
                worksheet.Cell(1, 9).Value = "Class";
                worksheet.Cell(1, 10).Value = "UoMAll";

                // Add sample data based on your database
                worksheet.Cell(2, 1).Value = "CBL";
                worksheet.Cell(2, 2).Value = 1.050;
                worksheet.Cell(2, 3).Value = 999999999999999.000;
                worksheet.Cell(2, 4).Value = 15.000;
                worksheet.Cell(2, 5).Value = 999999999999999.000;
                worksheet.Cell(2, 6).Value = 2.000;
                worksheet.Cell(2, 7).Value = 2.300;
                worksheet.Cell(2, 8).Value = 15;
                worksheet.Cell(2, 9).Value = "ClassB";
                worksheet.Cell(2, 10).Value = "KG";

                // Format header
                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save to stream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                // Return the file
                return new FileStreamResult(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "ProdClassification_Template.xlsx"
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = $"Error creating template: {ex.Message}" });
            }
        }

        /// <summary>
        /// Download Excel template for PortClassification
        /// </summary>
        [HttpGet("download-template/port-classification")]
        public IActionResult DownloadPortClassificationTemplate()
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("PortClassification");

                // Add headers
                worksheet.Cell(1, 1).Value = "PortNumber";
                worksheet.Cell(1, 2).Value = "Class";

                // Add sample data
                worksheet.Cell(2, 1).Value = "COM10";
                worksheet.Cell(2, 2).Value = "ClassA";
                worksheet.Cell(3, 1).Value = "COM11";
                worksheet.Cell(3, 2).Value = "ClassB";

                // Format header
                var headerRange = worksheet.Range("A1:B1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // Auto-fit columns
                worksheet.Columns().AdjustToContents();

                // Save to stream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                // Return the file
                return new FileStreamResult(stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    FileDownloadName = "PortClassification_Template.xlsx"
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = $"Error creating template: {ex.Message}" });
            }
        }
    }
}
