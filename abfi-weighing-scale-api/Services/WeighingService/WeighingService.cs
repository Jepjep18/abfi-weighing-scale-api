using abfi_weighing_scale_api.Controllers.WeighingDetailsController;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Models.Entities;
using abfi_weighing_scale_api.Services.WeighingDataProcessorService;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace abfi_weighing_scale_api.Services.WeighingService
{
    public interface IWeighingService
    {
        Task<WeighingDetailDto> SaveWeighingDataAsync(WeighingRequestDto request);
        Task<IEnumerable<WeighingDetailDto>> GetAllWeighingDataAsync();
        Task<WeighingDetailDto> GetWeighingDataByIdAsync(int id);
        Task<IEnumerable<WeighingDetailDto>> GetWeighingDataByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<UploadResultDto> UploadWeighingDetailsAsync(IFormFile file);
        Task<UploadResultDto> UploadProdClassificationAsync(IFormFile file);
        Task<UploadResultDto> UploadPortClassificationAsync(IFormFile file);
    }

    public class WeighingService : IWeighingService
    {
        private readonly AppDbContext _context;
        private readonly IWeighingDataProcessor _processor;

        public WeighingService(AppDbContext context, IWeighingDataProcessor processor)
        {
            _context = context;
            _processor = processor;
        }

        public async Task<WeighingDetailDto> SaveWeighingDataAsync(WeighingRequestDto request)
        {
            // Process the serial data
            var processedData = await _processor.ProcessSerialDataAsync(request.SerialData, request.PortNumber);

            // Create weighing detail entity
            var weighingDetail = new WeighingDetail
            {
                SerialData = request.SerialData,
                Qty = processedData.Qty,
                UoM = processedData.UoM,
                Heads = processedData.NumHeads,
                ProdCode = processedData.ProdCode,
                CreatedDateTime = DateTime.Now,
                PortNumber = request.PortNumber,
                Class = processedData.Class,
                Remarks = processedData.Remarks
            };

            // Save to database
            _context.WeighingDetails.Add(weighingDetail);
            await _context.SaveChangesAsync();

            // Map to DTO
            return MapToDto(weighingDetail);
        }

        public async Task<IEnumerable<WeighingDetailDto>> GetAllWeighingDataAsync()
        {
            var weighingDetails = await _context.WeighingDetails
                .OrderByDescending(w => w.CreatedDateTime)
                .ToListAsync();

            return weighingDetails.Select(MapToDto);
        }

        public async Task<WeighingDetailDto> GetWeighingDataByIdAsync(int id)
        {
            var weighingDetail = await _context.WeighingDetails.FindAsync(id);

            if (weighingDetail == null)
                return null;

            return MapToDto(weighingDetail);
        }

        public async Task<IEnumerable<WeighingDetailDto>> GetWeighingDataByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var weighingDetails = await _context.WeighingDetails
                .Where(w => w.CreatedDateTime >= startDate && w.CreatedDateTime <= endDate)
                .OrderByDescending(w => w.CreatedDateTime)
                .ToListAsync();

            return weighingDetails.Select(MapToDto);
        }

        private WeighingDetailDto MapToDto(WeighingDetail entity)
        {
            return new WeighingDetailDto
            {
                Id = entity.id,
                SerialData = entity.SerialData,
                Qty = entity.Qty,
                UoM = entity.UoM,
                Heads = entity.Heads,
                ProdCode = entity.ProdCode,
                CreatedDateTime = entity.CreatedDateTime,
                PortNumber = entity.PortNumber,
                Class = entity.Class,
                Remarks = entity.Remarks
            };
        }

        public async Task<UploadResultDto> UploadWeighingDetailsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new UploadResultDto { Success = false, Message = "Invalid file.", Count = 0 };

            var items = new List<WeighingDetail>();
            var errors = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var headerRow = worksheet.FirstRowUsed();
                var dataRows = worksheet.RowsUsed().Skip(1);

                // Create a dictionary to map column headers to indices
                var columnMap = new Dictionary<string, int>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    var header = cell.GetValue<string>()?.Trim().ToUpper();
                    if (!string.IsNullOrEmpty(header))
                    {
                        columnMap[header] = cell.Address.ColumnNumber;
                    }
                }

                int rowNum = 2; // Start from row 2 (after header)
                foreach (var row in dataRows)
                {
                    try
                    {
                        var serialData = GetCellValue(row, "SERIALDATA", columnMap)?.Trim();
                        var portNumber = GetCellValue(row, "PORTNUMBER", columnMap)?.Trim();

                        // Parse quantity
                        decimal? qty = null;
                        var qtyValue = GetCellValue(row, "QTY", columnMap);
                        if (decimal.TryParse(qtyValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedQty))
                            qty = parsedQty;

                        // Parse heads
                        int? heads = null;
                        var headsValue = GetCellValue(row, "HEADS", columnMap);
                        if (int.TryParse(headsValue, out var parsedHeads))
                            heads = parsedHeads;

                        // Parse date
                        DateTime? createdDateTime = null;
                        var dateValue = GetCellValue(row, "CREATEDDATETIME", columnMap);
                        if (DateTime.TryParse(dateValue, out var parsedDate))
                            createdDateTime = parsedDate;

                        var weighingDetail = new WeighingDetail
                        {
                            SerialData = serialData,
                            Qty = qty,
                            UoM = GetCellValue(row, "UOM", columnMap)?.Trim(),
                            Heads = heads,
                            ProdCode = GetCellValue(row, "PRODCODE", columnMap)?.Trim(),
                            CreatedDateTime = createdDateTime ?? DateTime.Now,
                            PortNumber = portNumber,
                            Class = GetCellValue(row, "CLASS", columnMap)?.Trim(),
                            Remarks = GetCellValue(row, "REMARKS", columnMap)?.Trim()
                        };

                        items.Add(weighingDetail);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNum}: {ex.Message}");
                    }

                    rowNum++;
                }

                if (items.Any())
                {
                    await _context.WeighingDetails.AddRangeAsync(items);
                    await _context.SaveChangesAsync();
                }

                return new UploadResultDto
                {
                    Success = true,
                    Message = errors.Any()
                        ? $"Upload completed with {errors.Count} errors."
                        : "Upload successful",
                    Count = items.Count,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Error processing Excel file: {ex.Message}",
                    Count = 0
                };
            }
        }

        public async Task<UploadResultDto> UploadProdClassificationAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new UploadResultDto { Success = false, Message = "Invalid file.", Count = 0 };

            var items = new List<ProdClassification>();
            var errors = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var headerRow = worksheet.FirstRowUsed();
                var dataRows = worksheet.RowsUsed().Skip(1);

                // Create a dictionary to map column headers to indices
                var columnMap = new Dictionary<string, int>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    var header = cell.GetValue<string>()?.Trim().ToUpper();
                    if (!string.IsNullOrEmpty(header))
                    {
                        columnMap[header] = cell.Address.ColumnNumber;
                    }
                }

                int rowNum = 2;
                foreach (var row in dataRows)
                {
                    try
                    {
                        // Parse all numeric values
                        decimal? indvWeightMin = ParseDecimal(GetCellValue(row, "INDVWEIGHT_MIN", columnMap));
                        decimal? indvWeightMax = ParseDecimal(GetCellValue(row, "INDVWEIGHT_MAX", columnMap));
                        decimal? totalIndvWeightMin = ParseDecimal(GetCellValue(row, "TOTALINDVWEIGHT_MIN", columnMap));
                        decimal? totalIndvWeightMax = ParseDecimal(GetCellValue(row, "TOTALINDVWEIGHT_MAX", columnMap));
                        decimal? cratesWeightMin = ParseDecimal(GetCellValue(row, "CRATESWEIGHT_MIN", columnMap));
                        decimal? cratesWeightMax = ParseDecimal(GetCellValue(row, "CRATESWEIGHT_MAX", columnMap));

                        // Parse NumHeads
                        int? numHeads = null;
                        var numHeadsValue = GetCellValue(row, "NUMHEADS", columnMap);
                        if (int.TryParse(numHeadsValue, out var parsedHeads))
                            numHeads = parsedHeads;

                        var prodClassification = new ProdClassification
                        {
                            ProdCode = GetCellValue(row, "PRODCODE", columnMap)?.Trim(),
                            IndvWeight_Min = indvWeightMin,
                            IndvWeight_Max = indvWeightMax,
                            TotalIndvWeight_Min = totalIndvWeightMin,
                            TotalIndvWeight_Max = totalIndvWeightMax,
                            CratesWeight_Min = cratesWeightMin,
                            CratesWeight_Max = cratesWeightMax,
                            NumHeads = numHeads,
                            Class = GetCellValue(row, "CLASS", columnMap)?.Trim(),
                            UoMAll = GetCellValue(row, "UOMALL", columnMap)?.Trim()
                        };

                        items.Add(prodClassification);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNum}: {ex.Message}");
                    }

                    rowNum++;
                }

                if (items.Any())
                {
                    await _context.ProdClassifications.AddRangeAsync(items);
                    await _context.SaveChangesAsync();
                }

                return new UploadResultDto
                {
                    Success = true,
                    Message = errors.Any()
                        ? $"Upload completed with {errors.Count} errors."
                        : "Upload successful",
                    Count = items.Count,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Error processing Excel file: {ex.Message}",
                    Count = 0
                };
            }
        }

        public async Task<UploadResultDto> UploadPortClassificationAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new UploadResultDto { Success = false, Message = "Invalid file.", Count = 0 };

            var items = new List<PortClassification>();
            var errors = new List<string>();

            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var headerRow = worksheet.FirstRowUsed();
                var dataRows = worksheet.RowsUsed().Skip(1);

                // Create a dictionary to map column headers to indices
                var columnMap = new Dictionary<string, int>();
                foreach (var cell in headerRow.CellsUsed())
                {
                    var header = cell.GetValue<string>()?.Trim().ToUpper();
                    if (!string.IsNullOrEmpty(header))
                    {
                        columnMap[header] = cell.Address.ColumnNumber;
                    }
                }

                int rowNum = 2;
                foreach (var row in dataRows)
                {
                    try
                    {
                        var portClassification = new PortClassification
                        {
                            PortNumber = GetCellValue(row, "PORTNUMBER", columnMap)?.Trim(),
                            Class = GetCellValue(row, "CLASS", columnMap)?.Trim()
                        };

                        items.Add(portClassification);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNum}: {ex.Message}");
                    }

                    rowNum++;
                }

                if (items.Any())
                {
                    await _context.PortClassifications.AddRangeAsync(items);
                    await _context.SaveChangesAsync();
                }

                return new UploadResultDto
                {
                    Success = true,
                    Message = errors.Any()
                        ? $"Upload completed with {errors.Count} errors."
                        : "Upload successful",
                    Count = items.Count,
                    Errors = errors
                };
            }
            catch (Exception ex)
            {
                return new UploadResultDto
                {
                    Success = false,
                    Message = $"Error processing Excel file: {ex.Message}",
                    Count = 0
                };
            }
        }

        // Helper method to get cell value
        private string GetCellValue(IXLRow row, string columnName, Dictionary<string, int> columnMap)
        {
            if (columnMap.TryGetValue(columnName, out int columnIndex))
            {
                var cell = row.Cell(columnIndex);
                return cell.Value.ToString();
            }
            return null;
        }

        // Helper method to parse decimal
        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            return null;
        }
    }
}
