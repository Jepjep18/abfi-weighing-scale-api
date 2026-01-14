using abfi_weighing_scale_api.Controllers.ProductClassification;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Models.Entities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace abfi_weighing_scale_api.Services.ProductClassifications
{
    // Service interface
    public interface IProductClassificationService
    {
        // Existing methods
        Task<(bool Success, string Message, int Count)> UploadAsync(IFormFile file);
        Task<List<ProductClassificationListDto>> GetAllAsync();

        // New CRUD methods
        Task<ProductClassificationDto> GetByIdAsync(int id);
        Task<List<ProductClassificationDto>> GetActiveAsync();
        Task<ProductClassificationDto> CreateAsync(CreateProductClassificationDto dto);
        Task<ProductClassificationDto> UpdateAsync(int id, UpdateProductClassificationDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> ToggleActiveStatusAsync(int id);
        Task<bool> ExistsByProductCodeAsync(string productCode, int? excludeId = null);
        Task<PagedResponseDto<ProductClassificationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string search = null);
        Task<List<ProductClassificationDto>> GetAllDetailsAsync();
    }



    // Service implementation
    public class ProductClassificationService : IProductClassificationService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductClassificationService> _logger;

        public ProductClassificationService(AppDbContext context, ILogger<ProductClassificationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Existing methods
        public async Task<(bool Success, string Message, int Count)> UploadAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "Invalid file.", 0);

            var items = new List<ProductClassification>();

            try
            {
                _logger.LogInformation("Starting product classification upload...");

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

                int recordCount = 0;
                foreach (var row in dataRows)
                {
                    recordCount++;

                    // Parse NoOfHeadsPerGalantina safely
                    int? noOfHeads = null;
                    var noOfHeadsValue = GetCellValue(row, "NO. HEADS PER GALANTINA", columnMap);
                    if (int.TryParse(noOfHeadsValue, out var heads))
                        noOfHeads = heads;

                    var productCode = GetCellValue(row, "PRODUCT CODE", columnMap);

                    // Skip if product code is empty
                    if (string.IsNullOrWhiteSpace(productCode))
                    {
                        _logger.LogWarning($"Row {recordCount}: Skipping due to empty product code");
                        continue;
                    }

                    // Check if product code already exists
                    var exists = await _context.ProductClassification
                        .AnyAsync(pc => pc.ProductCode == productCode);

                    if (exists)
                    {
                        _logger.LogWarning($"Row {recordCount}: Product code '{productCode}' already exists, skipping");
                        continue;
                    }

                    items.Add(new Models.Entities.ProductClassification
                    {
                        ProductCode = productCode,
                        IndividualWeightRange = GetCellValue(row, "INDIVIDUAL WEIGHT RANGE", columnMap),
                        TotalWeightRangePerCrate = GetCellValue(row, "TOTAL WEIGHT RANGE PER CRATES", columnMap),
                        NoOfHeadsPerGalantina = noOfHeads,
                        CratesWeight = GetCellValue(row, "CRATES WEIGHT", columnMap),
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                if (items.Any())
                {
                    await _context.ProductClassification.AddRangeAsync(items);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully uploaded {items.Count} product classifications");
                    return (true, "Upload successful", items.Count);
                }
                else
                {
                    _logger.LogWarning("No valid records found in the uploaded file");
                    return (false, "No valid records found in the file.", 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Excel file");
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

        // New CRUD methods
        public async Task<ProductClassificationDto> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting product classification by ID: {id}");

                var classification = await _context.ProductClassification
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (classification == null)
                {
                    _logger.LogWarning($"Product classification with ID {id} not found");
                    return null;
                }

                return MapToDto(classification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product classification by ID: {id}");
                throw;
            }
        }

        public async Task<List<ProductClassificationDto>> GetAllDetailsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all product classifications with details");

                var classifications = await _context.ProductClassification
                    .AsNoTracking()
                    .OrderBy(pc => pc.ProductCode)
                    .ToListAsync();

                return classifications.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product classifications with details");
                throw;
            }
        }

        public async Task<List<ProductClassificationDto>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Getting active product classifications");

                var classifications = await _context.ProductClassification
                    .AsNoTracking()
                    .Where(pc => pc.IsActive == true)
                    .OrderBy(pc => pc.ProductCode)
                    .ToListAsync();

                return classifications.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active product classifications");
                throw;
            }
        }

        public async Task<ProductClassificationDto> CreateAsync(CreateProductClassificationDto dto)
        {
            try
            {
                _logger.LogInformation($"Creating new product classification with code: {dto.ProductCode}");

                // Check if product code already exists
                var exists = await _context.ProductClassification
                    .AnyAsync(pc => pc.ProductCode == dto.ProductCode);

                if (exists)
                {
                    throw new InvalidOperationException($"Product code '{dto.ProductCode}' already exists");
                }

                var classification = new ProductClassification
                {
                    ProductCode = dto.ProductCode?.Trim(),
                    IndividualWeightRange = dto.IndividualWeightRange?.Trim(),
                    TotalWeightRangePerCrate = dto.TotalWeightRangePerCrate?.Trim(),
                    NoOfHeadsPerGalantina = dto.NoOfHeadsPerGalantina,
                    CratesWeight = dto.CratesWeight?.Trim(),
                    IsActive = dto.IsActive ?? true,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.ProductClassification.AddAsync(classification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product classification created with ID: {classification.Id}");
                return MapToDto(classification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating product classification with code: {dto.ProductCode}");
                throw;
            }
        }

        public async Task<ProductClassificationDto> UpdateAsync(int id, UpdateProductClassificationDto dto)
        {
            try
            {
                _logger.LogInformation($"Updating product classification with ID: {id}");

                var classification = await _context.ProductClassification
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (classification == null)
                {
                    throw new KeyNotFoundException($"Product classification with ID {id} not found");
                }

                // Check if product code already exists (excluding current record)
                // Only check if productCode is provided and different from current
                if (dto.ProductCode != null && dto.ProductCode != classification.ProductCode)
                {
                    var exists = await _context.ProductClassification
                        .AnyAsync(pc => pc.ProductCode == dto.ProductCode && pc.Id != id);

                    if (exists)
                    {
                        throw new InvalidOperationException($"Product code '{dto.ProductCode}' already exists");
                    }
                }

                // PRODUCT CODE: Update if provided (including empty string)
                if (dto.ProductCode != null)
                    classification.ProductCode = dto.ProductCode.Trim();

                // INDIVIDUAL WEIGHT RANGE: 
                // - null: don't update (keep existing)
                // - empty string: update to empty/clear it
                // - has value: update with new value
                if (dto.IndividualWeightRange != null)
                    classification.IndividualWeightRange = string.IsNullOrWhiteSpace(dto.IndividualWeightRange)
                        ? string.Empty
                        : dto.IndividualWeightRange.Trim();

                // TOTAL WEIGHT RANGE PER CRATE:
                if (dto.TotalWeightRangePerCrate != null)
                    classification.TotalWeightRangePerCrate = string.IsNullOrWhiteSpace(dto.TotalWeightRangePerCrate)
                        ? string.Empty
                        : dto.TotalWeightRangePerCrate.Trim();

                // NO OF HEADS PER GALANTINA:
                // - null: don't update (keep existing)
                // - has value: update with new value (0 is valid)
                if (dto.NoOfHeadsPerGalantina.HasValue)
                    classification.NoOfHeadsPerGalantina = dto.NoOfHeadsPerGalantina.Value;

                // CRATES WEIGHT:
                if (dto.CratesWeight != null)
                    classification.CratesWeight = string.IsNullOrWhiteSpace(dto.CratesWeight)
                        ? string.Empty
                        : dto.CratesWeight.Trim();

                // IS ACTIVE:
                // - null: don't update (keep existing)
                // - has value: update with new value
                if (dto.IsActive.HasValue)
                    classification.IsActive = dto.IsActive.Value;

                classification.LastUpdatedAt = DateTime.UtcNow;

                _context.ProductClassification.Update(classification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product classification updated with ID: {id}");
                return MapToDto(classification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product classification with ID: {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting product classification with ID: {id}");

                var classification = await _context.ProductClassification
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (classification == null)
                {
                    _logger.LogWarning($"Product classification with ID {id} not found for deletion");
                    return false;
                }

                // Check if there are related booking items
                var hasRelatedItems = await _context.BookingItems
                    .AnyAsync(bi => bi.ProductClassificationId == id);

                if (hasRelatedItems)
                {
                    throw new InvalidOperationException(
                        "Cannot delete product classification because it has related booking items. Use soft delete instead.");
                }

                _context.ProductClassification.Remove(classification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product classification deleted with ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product classification with ID: {id}");
                throw;
            }
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Soft deleting product classification with ID: {id}");

                var classification = await _context.ProductClassification
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (classification == null)
                {
                    _logger.LogWarning($"Product classification with ID {id} not found for soft deletion");
                    return false;
                }

                classification.IsActive = false;
                classification.LastUpdatedAt = DateTime.UtcNow;

                _context.ProductClassification.Update(classification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product classification soft deleted with ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting product classification with ID: {id}");
                throw;
            }
        }

        public async Task<bool> ToggleActiveStatusAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Toggling active status for product classification with ID: {id}");

                var classification = await _context.ProductClassification
                    .FirstOrDefaultAsync(pc => pc.Id == id);

                if (classification == null)
                {
                    _logger.LogWarning($"Product classification with ID {id} not found for status toggle");
                    return false;
                }

                classification.IsActive = !(classification.IsActive ?? false);
                classification.LastUpdatedAt = DateTime.UtcNow;

                _context.ProductClassification.Update(classification);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Product classification status toggled to {classification.IsActive} for ID: {id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling active status for product classification with ID: {id}");
                throw;
            }
        }

        public async Task<bool> ExistsByProductCodeAsync(string productCode, int? excludeId = null)
        {
            try
            {
                var query = _context.ProductClassification
                    .Where(pc => pc.ProductCode == productCode);

                if (excludeId.HasValue)
                {
                    query = query.Where(pc => pc.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if product code exists: {productCode}");
                throw;
            }
        }

        public async Task<PagedResponseDto<ProductClassificationDto>> GetPagedAsync(int pageNumber = 1, int pageSize = 10, string search = null)
        {
            try
            {
                _logger.LogInformation($"Getting paged product classifications - Page: {pageNumber}, Size: {pageSize}");

                // Validate pagination
                pageNumber = Math.Max(1, pageNumber);
                pageSize = Math.Max(1, Math.Min(pageSize, 100)); // Limit to 100 per page

                var query = _context.ProductClassification
                    .AsNoTracking()
                    .AsQueryable();

                // Apply search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchTerm = search.Trim().ToLower();
                    query = query.Where(pc =>
                        (pc.ProductCode != null && pc.ProductCode.ToLower().Contains(searchTerm)) ||
                        (pc.IndividualWeightRange != null && pc.IndividualWeightRange.ToLower().Contains(searchTerm)) ||
                        (pc.TotalWeightRangePerCrate != null && pc.TotalWeightRangePerCrate.ToLower().Contains(searchTerm))
                    );
                }

                // Get total count
                var totalCount = await query.CountAsync();

                // Get paginated data
                var classifications = await query
                    .OrderBy(pc => pc.ProductCode)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var items = classifications.Select(MapToDto).ToList();

                return new PagedResponseDto<ProductClassificationDto>
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Items = items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged product classifications");
                throw;
            }
        }

        // Helper method to map entity to DTO
        private ProductClassificationDto MapToDto(ProductClassification entity)
        {
            return new ProductClassificationDto
            {
                Id = entity.Id,
                ProductCode = entity.ProductCode ?? string.Empty,
                IndividualWeightRange = entity.IndividualWeightRange ?? string.Empty,
                TotalWeightRangePerCrate = entity.TotalWeightRangePerCrate ?? string.Empty,
                NoOfHeadsPerGalantina = entity.NoOfHeadsPerGalantina ?? 0,
                CratesWeight = entity.CratesWeight ?? string.Empty,
                IsActive = entity.IsActive ?? false,
                //CreatedAt = entity.CreatedAt,
                //LastUpdatedAt = entity.LastUpdatedAt
            };
        }
    }
}