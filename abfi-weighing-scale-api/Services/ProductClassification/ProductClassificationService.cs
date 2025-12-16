using abfi_weighing_scale_api.Controllers.ProductClassification;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Models.Entities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace abfi_weighing_scale_api.Services.ProductClassifications
{
    // Service interface
    public interface IProductClassificationService
    {
        Task<(bool Success, string Message, int Count)> UploadAsync(IFormFile file);
        Task<List<ProductClassificationListDto>> GetAllAsync();

    }

    // Service implementation
    public class ProductClassificationService : IProductClassificationService
    {
        private readonly AppDbContext _context;

        public ProductClassificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, int Count)> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "Invalid file.", 0);

            var items = new List<ProductClassification>();

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

                foreach (var row in dataRows)
                {
                    // Parse NoOfHeadsPerGalantina safely
                    int? noOfHeads = null;
                    var noOfHeadsValue = GetCellValue(row, "NO. HEADS PER GALANTINA", columnMap);
                    if (int.TryParse(noOfHeadsValue, out var heads))
                        noOfHeads = heads;

                    items.Add(new Models.Entities.ProductClassification
                    {
                        ProductCode = GetCellValue(row, "PRODUCT CODE", columnMap),
                        IndividualWeightRange = GetCellValue(row, "INDIVIDUAL WEIGHT RANGE", columnMap),
                        TotalWeightRangePerCrate = GetCellValue(row, "TOTAL WEIGHT RANGE PER CRATES", columnMap),
                        NoOfHeadsPerGalantina = noOfHeads,
                        CratesWeight = GetCellValue(row, "CRATES WEIGHT", columnMap),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                await _context.ProductClassification.AddRangeAsync(items);
                await _context.SaveChangesAsync();

                return (true, "Upload successful", items.Count);
            }
            catch (Exception ex)
            {
                return (false, $"Error processing Excel file: {ex.Message}", 0);
            }
        }

        private string GetCellValue(IXLRow row, string header, Dictionary<string, int> columnMap)
        {
            if (columnMap.TryGetValue(header.ToUpper(), out int columnIndex))
            {
                return row.Cell(columnIndex).GetValue<string>()?.Trim();
            }
            return null;
        }

        public async Task<List<ProductClassificationListDto>> GetAllAsync()
        {
            return await _context.ProductClassification
                .AsNoTracking()
                .Select(x => new ProductClassificationListDto
                {
                    Id = x.Id,
                    ProductCode = x.ProductCode
                })
                .OrderBy(x => x.ProductCode)
                .ToListAsync();
        }
    }
}
