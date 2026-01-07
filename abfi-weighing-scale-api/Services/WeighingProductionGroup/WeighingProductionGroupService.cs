using abfi_weighing_scale_api.Controllers.Production;
using abfi_weighing_scale_api.Data;
using abfi_weighing_scale_api.Helpers;
using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Models.DTOs.WeighingProductionGroup;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionEntity = abfi_weighing_scale_api.Models.Entities.Production;
using WeighingProductionGroupEntity = abfi_weighing_scale_api.Models.Entities.WeighingProductionGroup;

namespace abfi_weighing_scale_api.Services
{
    public interface IWeighingProductionGroupService
    {
        Task<WeighingProductionGroupResponseDto> CreateAsync(CreateWeighingProductionGroupDto dto);
        Task<WeighingProductionGroupResponseDto?> GetByIdAsync(int id);
        Task<PagedResponseDto<WeighingProductionGroupResponseDto>> GetAllAsync(
                ProductionRequestDto request
            );
    }

    public class WeighingProductionGroupService : IWeighingProductionGroupService
    {
        private readonly AppDbContext _context;

        public WeighingProductionGroupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WeighingProductionGroupResponseDto> CreateAsync(CreateWeighingProductionGroupDto dto)
        {
            // Validate that Production exists
            var production = await _context.Production
                .FirstOrDefaultAsync(p => p.Id == dto.ProductionId);

            if (production == null)
            {
                throw new ArgumentException($"Production with ID {dto.ProductionId} does not exist.");
            }

            var weighingProductionGroup = new WeighingProductionGroupEntity
            {
                ProductionId = dto.ProductionId,
                CreatedAt = TimeHelper.GetPhilippineStandardTime()
            };

            _context.WeighingProductionGroups.Add(weighingProductionGroup);
            await _context.SaveChangesAsync();

            // Reload with Production data
            await _context.Entry(weighingProductionGroup)
                .Reference(wpg => wpg.Production)
                .LoadAsync();

            return MapToDto(weighingProductionGroup);
        }

        public async Task<WeighingProductionGroupResponseDto?> GetByIdAsync(int id)
        {
            var group = await _context.WeighingProductionGroups
                .Include(wpg => wpg.Production)
                    .ThenInclude(p => p.ProductionFarms)
                        .ThenInclude(pf => pf.Farm)
                .FirstOrDefaultAsync(wpg => wpg.Id == id);

            if (group == null) return null;

            return MapToDto(group);
        }

        public async Task<PagedResponseDto<WeighingProductionGroupResponseDto>> GetAllAsync(
    ProductionRequestDto request
)
        {
            var query = _context.WeighingProductionGroups
                .Include(wpg => wpg.Production)
                    .ThenInclude(p => p.ProductionFarms)
                        .ThenInclude(pf => pf.Farm)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLower();

                query = query.Where(wpg =>
                    wpg.Production.ProductionName.ToLower().Contains(searchTerm) ||
                    wpg.Production.ProductionFarms.Any(pf =>
                        pf.Farm != null &&
                        pf.Farm.FarmName.ToLower().Contains(searchTerm)
                    )
                );
            }

            // Total count for pagination
            var totalCount = await query.CountAsync();

            // Ordering
            query = query.OrderByDescending(wpg => wpg.CreatedAt);

            // Pagination
            var groups = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var items = groups.Select(MapToDto);

            return new PagedResponseDto<WeighingProductionGroupResponseDto>
            {
                CurrentPage = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
                Items = items
            };
        }


        private WeighingProductionGroupResponseDto MapToDto(WeighingProductionGroupEntity entity)
        {
            return new WeighingProductionGroupResponseDto
            {
                Id = entity.Id,
                ProductionId = entity.ProductionId,
                CreatedAt = entity.CreatedAt,
                Production = entity.Production != null ? new ProductionInfoDto
                {
                    Id = entity.Production.Id,
                    ProductionName = entity.Production.ProductionName,
                    TotalHeads = entity.Production.TotalHeads ?? 0, // Handle nullable int
                    StartDateTime = entity.Production.StartDateTime,
                    EndDateTime = entity.Production.EndDateTime,
                    Description = entity.Production.Description,
                    FarmDetails = entity.Production.ProductionFarms?.Select(pf => new Models.DTOs.WeighingProductionGroup.ProductionFarmDetailDto
                    {
                        Id = pf.Id,
                        ProductionId = pf.ProductionId,
                        FarmId = pf.FarmId,
                        FarmName = pf.Farm?.FarmName ?? "Unknown",
                        ForecastedTrips = pf.ForecastedTrips,
                        AllocatedHeads = pf.AllocatedHeads,
                        CreatedAt = pf.CreatedAt
                    }).ToList() ?? new List<Models.DTOs.WeighingProductionGroup.ProductionFarmDetailDto>()
                } : null
            };
        }
    }
}