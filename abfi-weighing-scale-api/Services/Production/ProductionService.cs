// Services/Production/ProductionService.cs
using abfi_weighing_scale_api.Controllers.Production;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Helpers;
using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace abfi_weighing_scale_api.Services.Production
{
    public interface IProductionService
    {
        Task<PagedResponseDto<ProductionListDto>> GetProductionsAsync(ProductionRequestDto request);
        Task<ProductionDto?> GetProductionByIdAsync(int id);
        Task<ProductionDto> CreateProductionAsync(CreateProductionDto createProductionDto);
        Task<ProductionDto?> UpdateProductionAsync(int id, UpdateProductionDto updateProductionDto);
        Task<bool> DeleteProductionAsync(int id);
        Task<IEnumerable<ProductionListDto>> GetProductionsByFarmIdAsync(int farmId);
        Task<ProductionDto?> AddFarmsToProductionAsync(int productionId, List<ProductionFarmCreateDto> farmDetails);
        Task<ProductionDto?> RemoveFarmFromProductionAsync(int productionId, int farmId);
    }

    public class ProductionService : IProductionService
    {
        private readonly AppDbContext _context;

        public ProductionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponseDto<ProductionListDto>> GetProductionsAsync(ProductionRequestDto request)
        {
            var query = _context.Production
                .Include(p => p.ProductionFarms)
                    .ThenInclude(pf => pf.Farm)
                .AsQueryable();

            // Apply search filter if search term is provided
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();
                query = query.Where(p =>
                    p.ProductionName.ToLower().Contains(searchTerm) ||
                    p.ProductionFarms.Any(pf => pf.Farm != null && pf.Farm.FarmName.ToLower().Contains(searchTerm))
                );
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply ordering
            query = query.OrderByDescending(p => p.CreatedAt);

            // Apply pagination
            var productions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = productions.Select(p => new ProductionListDto
            {
                Id = p.Id,
                ProductionName = p.ProductionName,
                TotalHeads = p.TotalHeads,
                StartDateTime = p.StartDateTime,
                EndDateTime = p.EndDateTime,
                CreatedAt = p.CreatedAt,
                FarmCount = p.ProductionFarms.Count,
                //FarmNames = p.ProductionFarms.Select(pf => pf.Farm?.FarmName ?? "Unknown").ToList(),
                // Add farm details using existing ProductionFarmDetailDto
                FarmDetails = p.ProductionFarms.Select(pf => new ProductionFarmDetailDto
                {
                    Id = pf.Id,
                    ProductionId = pf.ProductionId,
                    FarmId = pf.FarmId,
                    FarmName = pf.Farm?.FarmName ?? "Unknown",
                    ForecastedTrips = pf.ForecastedTrips,
                    AllocatedHeads = pf.AllocatedHeads,
                    CreatedAt = pf.CreatedAt
                }).ToList()
            });

            return new PagedResponseDto<ProductionListDto>
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };
        }

        public async Task<ProductionDto?> GetProductionByIdAsync(int id)
        {
            var production = await _context.Production
                .Include(p => p.ProductionFarms)
                    .ThenInclude(pf => pf.Farm)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (production == null) return null;

            return new ProductionDto
            {
                Id = production.Id,
                ProductionName = production.ProductionName,
                TotalHeads = production.TotalHeads,
                StartDateTime = production.StartDateTime,
                EndDateTime = production.EndDateTime,
                CreatedAt = production.CreatedAt,
                Description = production.Description,
                FarmDetails = production.ProductionFarms.Select(pf => new ProductionFarmDetailDto
                {
                    Id = pf.Id,
                    ProductionId = pf.ProductionId,
                    FarmId = pf.FarmId,
                    FarmName = pf.Farm?.FarmName,
                    ForecastedTrips = pf.ForecastedTrips,
                    AllocatedHeads = pf.AllocatedHeads,
                    CreatedAt = pf.CreatedAt
                }).ToList()
            };
        }

        public async Task<IEnumerable<ProductionListDto>> GetProductionsByFarmIdAsync(int farmId)
        {
            var productions = await _context.Production
                .Include(p => p.ProductionFarms)
                    .ThenInclude(pf => pf.Farm)
                .Where(p => p.ProductionFarms.Any(pf => pf.FarmId == farmId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return productions.Select(p => new ProductionListDto
            {
                Id = p.Id,
                ProductionName = p.ProductionName,
                TotalHeads = p.TotalHeads,
                StartDateTime = p.StartDateTime,
                EndDateTime = p.EndDateTime,
                CreatedAt = p.CreatedAt,
                FarmCount = p.ProductionFarms.Count,
                //FarmNames = p.ProductionFarms.Select(pf => pf.Farm?.FarmName ?? "Unknown").ToList()
            });
        }

        public async Task<ProductionDto> CreateProductionAsync(CreateProductionDto createProductionDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Create the main Production record
                var production = new Models.Entities.Production
                {
                    ProductionName = createProductionDto.ProductionName,
                    TotalHeads = createProductionDto.TotalHeads,
                    StartDateTime = createProductionDto.StartDateTime,
                    //EndDateTime = createProductionDto.EndDateTime,
                    Description = createProductionDto.Description,
                    CreatedAt = TimeHelper.GetPhilippineStandardTime()
                };

                _context.Production.Add(production);
                await _context.SaveChangesAsync();

                // 2. Validate and add farm details if provided
                if (createProductionDto.FarmDetails != null && createProductionDto.FarmDetails.Any())
                {
                    await AddFarmsToProductionInternalAsync(production.Id, createProductionDto.FarmDetails);
                }

                await transaction.CommitAsync();

                // 3. Return the complete production with farm details
                return await GetProductionByIdAsync(production.Id);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ProductionDto?> UpdateProductionAsync(int id, UpdateProductionDto updateProductionDto)
        {
            var production = await _context.Production.FindAsync(id);

            if (production == null) return null;

            // Update properties if provided
            if (!string.IsNullOrEmpty(updateProductionDto.ProductionName))
                production.ProductionName = updateProductionDto.ProductionName;

            if (updateProductionDto.TotalHeads.HasValue)
                production.TotalHeads = updateProductionDto.TotalHeads.Value;

            if (updateProductionDto.StartDateTime.HasValue)
                production.StartDateTime = updateProductionDto.StartDateTime.Value;

            if (updateProductionDto.EndDateTime.HasValue)
            {
                // Validate end date is not before start date
                if (production.StartDateTime.HasValue &&
                    updateProductionDto.EndDateTime.Value < production.StartDateTime.Value)
                {
                    throw new ArgumentException("End date cannot be earlier than start date.");
                }
                production.EndDateTime = updateProductionDto.EndDateTime.Value;
            }

            if (!string.IsNullOrEmpty(updateProductionDto.Description))
                production.Description = updateProductionDto.Description;

            await _context.SaveChangesAsync();

            return await GetProductionByIdAsync(id);
        }

        public async Task<bool> DeleteProductionAsync(int id)
        {
            var production = await _context.Production.FindAsync(id);

            if (production == null) return false;

            _context.Production.Remove(production);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ProductionDto?> AddFarmsToProductionAsync(int productionId, List<ProductionFarmCreateDto> farmDetails)
        {
            var production = await _context.Production.FindAsync(productionId);
            if (production == null) return null;

            await AddFarmsToProductionInternalAsync(productionId, farmDetails);

            return await GetProductionByIdAsync(productionId);
        }

        public async Task<ProductionDto?> RemoveFarmFromProductionAsync(int productionId, int farmId)
        {
            var productionFarm = await _context.ProductionFarms
                .FirstOrDefaultAsync(pf => pf.ProductionId == productionId && pf.FarmId == farmId);

            if (productionFarm == null) return null;

            _context.ProductionFarms.Remove(productionFarm);
            await _context.SaveChangesAsync();

            return await GetProductionByIdAsync(productionId);
        }

        private async Task AddFarmsToProductionInternalAsync(int productionId, List<ProductionFarmCreateDto> farmDetails)
        {
            // Validate all farms exist and are active
            var farmIds = farmDetails.Select(f => f.FarmId).Distinct().ToList();
            var existingFarms = await _context.Farms
                .Where(f => farmIds.Contains(f.Id) && f.IsActive)
                .Select(f => f.Id)
                .ToListAsync();

            var missingOrInactiveFarms = farmIds.Except(existingFarms).ToList();
            if (missingOrInactiveFarms.Any())
            {
                throw new ArgumentException($"The following farm IDs do not exist or are inactive: {string.Join(", ", missingOrInactiveFarms)}");
            }

            // Check for duplicates in the request
            var duplicateFarmIds = farmDetails
                .GroupBy(f => f.FarmId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            if (duplicateFarmIds.Any())
            {
                throw new ArgumentException($"Duplicate farm IDs in request: {string.Join(", ", duplicateFarmIds)}");
            }

            // Check if any farms are already added to this production
            var existingProductionFarms = await _context.ProductionFarms
                .Where(pf => pf.ProductionId == productionId && farmIds.Contains(pf.FarmId))
                .Select(pf => pf.FarmId)
                .ToListAsync();

            if (existingProductionFarms.Any())
            {
                throw new ArgumentException($"The following farms are already added to this production: {string.Join(", ", existingProductionFarms)}");
            }

            // Create ProductionFarm records
            var productionFarms = farmDetails.Select(farmDetail => new ProductionFarm
            {
                ProductionId = productionId,
                FarmId = farmDetail.FarmId,
                ForecastedTrips = farmDetail.ForecastedTrips,
                AllocatedHeads = farmDetail.AllocatedHeads,
                CreatedAt = TimeHelper.GetPhilippineStandardTime()
            }).ToList();

            _context.ProductionFarms.AddRange(productionFarms);
            await _context.SaveChangesAsync();
        }
    }
}