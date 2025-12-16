// Services/Farms/FarmsService.cs
using abfi_weighing_scale_api.Controllers.Farms;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Models.Entities;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace abfi_weighing_scale_api.Services.Farms
{
    public interface IFarmsService
    {
        Task<IEnumerable<FarmDto>> GetAllFarmsAsync();
        Task<FarmDto?> GetFarmByIdAsync(int id);
        Task<FarmDto> CreateFarmAsync(CreateFarmDto createFarmDto);
        Task<FarmDto?> UpdateFarmAsync(int id, UpdateFarmDto updateFarmDto);
        Task<bool> DeleteFarmAsync(int id);
        Task<bool> ToggleFarmStatusAsync(int id);
        Task<(bool Success, string Message, int Count)> UploadFarmsAsync(IFormFile file);

    }

    public class FarmsService : IFarmsService
    {
        private readonly AppDbContext _context;

        public FarmsService(AppDbContext context)
        {
            _context = context;
        }

        //get all
        public async Task<IEnumerable<FarmDto>> GetAllFarmsAsync()
        {
            var farms = await _context.Farms
                .Where(f => f.IsActive)
                .OrderBy(f => f.FarmName)
                .ToListAsync();

            return farms.Select(f => new FarmDto
            {
                Id = f.Id,
                FarmName = f.FarmName,
                IsActive = f.IsActive,
                //CreatedAt = f.CreatedAt,
                //UpdatedAt = f.UpdatedAt
            });
        }


        //get by id
        public async Task<FarmDto?> GetFarmByIdAsync(int id)
        {
            var farm = await _context.Farms.FindAsync(id);

            if (farm == null) return null;

            return new FarmDto
            {
                Id = farm.Id,
                FarmName = farm.FarmName,
                IsActive = farm.IsActive,
                //CreatedAt = farm.CreatedAt,
                //UpdatedAt = farm.UpdatedAt
            };
        }


        //create farms
        public async Task<FarmDto> CreateFarmAsync(CreateFarmDto createFarmDto)
        {
            var farm = new Models.Entities.Farms
            {
                FarmName = createFarmDto.FarmName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            return new FarmDto
            {
                Id = farm.Id,
                FarmName = farm.FarmName,
                IsActive = farm.IsActive,
                //CreatedAt = farm.CreatedAt,
                //UpdatedAt = farm.UpdatedAt
            };
        }


        //update farm
        public async Task<FarmDto?> UpdateFarmAsync(int id, UpdateFarmDto updateFarmDto)
        {
            var farm = await _context.Farms.FindAsync(id);

            if (farm == null) return null;

            if (!string.IsNullOrEmpty(updateFarmDto.FarmName))
            {
                farm.FarmName = updateFarmDto.FarmName;
            }

            farm.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new FarmDto
            {
                Id = farm.Id,
                FarmName = farm.FarmName,
                IsActive = farm.IsActive,
                //CreatedAt = farm.CreatedAt,
                //UpdatedAt = farm.UpdatedAt
            };
        }


        //delete farm
        public async Task<bool> DeleteFarmAsync(int id)
        {
            var farm = await _context.Farms.FindAsync(id);

            if (farm == null) return false;

            _context.Farms.Remove(farm);
            await _context.SaveChangesAsync();

            return true;
        }

        //toggle status
        public async Task<bool> ToggleFarmStatusAsync(int id)
        {
            var farm = await _context.Farms.FindAsync(id);

            if (farm == null) return false;

            farm.IsActive = !farm.IsActive;
            farm.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        //upload endpoint
        public async Task<(bool Success, string Message, int Count)> UploadFarmsAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "Invalid file.", 0);

            var items = new List<Models.Entities.Farms>();

            try
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1); // first sheet

                // Skip the header row
                var rows = worksheet.RowsUsed().Skip(1);

                foreach (var row in rows)
                {
                    var farmName = row.Cell(1).GetValue<string>()?.Trim();
                    var isActiveValue = row.Cell(2).GetValue<string>()?.Trim();

                    // Parse IsActive - default to true if not specified or invalid
                    bool isActive = true;
                    if (!string.IsNullOrEmpty(isActiveValue))
                    {
                        if (int.TryParse(isActiveValue, out int isActiveInt))
                        {
                            isActive = isActiveInt == 1;
                        }
                        else if (bool.TryParse(isActiveValue, out bool isActiveBool))
                        {
                            isActive = isActiveBool;
                        }
                    }

                    // Validate required field
                    if (string.IsNullOrWhiteSpace(farmName))
                        continue; // Skip rows with empty farm name

                    items.Add(new Models.Entities.Farms
                    {
                        FarmName = farmName,
                        IsActive = isActive,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                // Check for duplicates before inserting
                var existingFarms = await _context.Farms
                    .Where(f => items.Select(i => i.FarmName.ToUpper()).Contains(f.FarmName.ToUpper()))
                    .Select(f => f.FarmName)
                    .ToListAsync();

                if (existingFarms.Any())
                {
                    return (false, $"The following farms already exist: {string.Join(", ", existingFarms)}", 0);
                }

                await _context.Farms.AddRangeAsync(items);
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