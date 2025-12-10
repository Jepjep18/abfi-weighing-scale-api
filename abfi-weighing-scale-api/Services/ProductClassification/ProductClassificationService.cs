using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Models.Entities;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http;
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
                var worksheet = workbook.Worksheet(1); // first sheet

                // Skip the header row
                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    // Parse NoOfHeadsPerGalantina safely
                    int? noOfHeads = null;
                    var noOfHeadsValue = row.Cell(4).GetValue<string>()?.Trim();
                    if (int.TryParse(noOfHeadsValue, out var heads))
                        noOfHeads = heads;

                    // CratesWeight as string
                    var cratesWeight = row.Cell(5).GetValue<string>()?.Trim();

                    items.Add(new Models.Entities.ProductClassification
                    {
                        ProductCode = row.Cell(1).GetValue<string>()?.Trim(),
                        IndividualWeightRange = row.Cell(2).GetValue<string>()?.Trim(),
                        TotalWeightRangePerCrate = row.Cell(3).GetValue<string>()?.Trim(),
                        NoOfHeadsPerGalantina = noOfHeads,
                        CratesWeight = cratesWeight,
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
    }
}
