using Microsoft.AspNetCore.Mvc;
using abfi_weighing_scale_api.Services.Production;

namespace abfi_weighing_scale_api.Controllers.Production
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionController : ControllerBase
    {
        private readonly IProductionService _productionService;

        public ProductionController(IProductionService productionService)
        {
            _productionService = productionService;
        }

        // GET: api/production
        [HttpGet]
        public async Task<IActionResult> GetProductions([FromQuery] ProductionRequestDto request)
        {
            // Validate pagination parameters
            request.PageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            request.PageSize = request.PageSize > 100 ? 100 : request.PageSize;
            request.PageSize = request.PageSize < 1 ? 10 : request.PageSize;

            var result = await _productionService.GetProductionsAsync(request);
            return Ok(result);
        }

        // GET: api/production/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductionById(int id)
        {
            var production = await _productionService.GetProductionByIdAsync(id);

            if (production == null)
                return NotFound(new { Message = $"Production with ID {id} not found" });

            return Ok(production);
        }

        // GET: api/production/farm/{farmId}
        [HttpGet("farm/{farmId}")]
        public async Task<IActionResult> GetProductionsByFarmId(int farmId)
        {
            var productions = await _productionService.GetProductionsByFarmIdAsync(farmId);
            return Ok(productions);
        }

        // POST: api/production
        [HttpPost]
        public async Task<IActionResult> CreateProduction([FromBody] CreateProductionDto createProductionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var production = await _productionService.CreateProductionAsync(createProductionDto);
                return CreatedAtAction(nameof(GetProductionById), new { id = production.Id }, production);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while creating production.", Details = ex.Message });
            }
        }

        // PUT: api/production/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduction(int id, [FromBody] UpdateProductionDto updateProductionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var production = await _productionService.UpdateProductionAsync(id, updateProductionDto);

                if (production == null)
                    return NotFound(new { Message = $"Production with ID {id} not found" });

                return Ok(production);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while updating production.", Details = ex.Message });
            }
        }

        // POST: api/production/{id}/farms
        [HttpPost("{id}/farms")]
        public async Task<IActionResult> AddFarmsToProduction(int id, [FromBody] List<ProductionFarmCreateDto> farmDetails)
        {
            if (!ModelState.IsValid || farmDetails == null || !farmDetails.Any())
                return BadRequest("Farm details are required.");

            try
            {
                var production = await _productionService.AddFarmsToProductionAsync(id, farmDetails);

                if (production == null)
                    return NotFound(new { Message = $"Production with ID {id} not found" });

                return Ok(production);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while adding farms to production.", Details = ex.Message });
            }
        }

        // DELETE: api/production/{id}/farms/{farmId}
        [HttpDelete("{id}/farms/{farmId}")]
        public async Task<IActionResult> RemoveFarmFromProduction(int id, int farmId)
        {
            try
            {
                var production = await _productionService.RemoveFarmFromProductionAsync(id, farmId);

                if (production == null)
                    return NotFound(new { Message = $"Production with ID {id} or farm with ID {farmId} not found" });

                return Ok(production);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while removing farm from production.", Details = ex.Message });
            }
        }

        // DELETE: api/production/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduction(int id)
        {
            var result = await _productionService.DeleteProductionAsync(id);

            if (!result)
                return NotFound(new { Message = $"Production with ID {id} not found" });

            return NoContent();
        }
    }
}