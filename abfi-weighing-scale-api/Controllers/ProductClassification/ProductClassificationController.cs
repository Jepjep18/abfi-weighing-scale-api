using abfi_weighing_scale_api.Services.ProductClassifications;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace abfi_weighing_scale_api.Controllers.ProductClassification
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductClassificationController : ControllerBase
    {
        private readonly IProductClassificationService _service;
        private readonly ILogger<ProductClassificationController> _logger;

        public ProductClassificationController(
            IProductClassificationService service,
            ILogger<ProductClassificationController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Upload a CSV/Excel file with product classification master data.
        /// CSV must have headers: PRODUCT CODE, INDIVIDUAL WEIGHT RANGE, TOTAL WEIGHT RANGE PER CRATE, NO. HEADS PER GALANTINA, CRATES WEIGHT
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ProductClassificationFileUploadDto dto)
        {
            try
            {
                _logger.LogInformation("Processing product classification upload");

                if (dto.File == null)
                    return BadRequest(new { Message = "No file uploaded." });

                var result = await _service.UploadAsync(dto.File);

                if (!result.Success)
                    return BadRequest(new { Message = result.Message });

                return Ok(new
                {
                    Message = result.Message,
                    RecordsInserted = result.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading product classification file");
                return StatusCode(500, new { Message = "An error occurred while processing the file." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product classifications");
                return StatusCode(500, new { Message = "An error occurred while retrieving data." });
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetAllDetails()
        {
            try
            {
                var result = await _service.GetAllDetailsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all product classification details");
                return StatusCode(500, new { Message = "An error occurred while retrieving data." });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var result = await _service.GetActiveAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active product classifications");
                return StatusCode(500, new { Message = "An error occurred while retrieving data." });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);

                if (result == null)
                    return NotFound(new { Message = $"Product classification with ID {id} not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product classification by ID: {id}");
                return StatusCode(500, new { Message = "An error occurred while retrieving data." });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int size = 10,
            [FromQuery] string search = null)
        {
            try
            {
                var result = await _service.GetPagedAsync(page, size, search);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting paged product classifications");
                return StatusCode(500, new { Message = "An error occurred while retrieving data." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductClassificationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product classification");
                return StatusCode(500, new { Message = "An error occurred while creating the record." });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductClassificationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _service.UpdateAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product classification with ID: {id}");
                return StatusCode(500, new { Message = "An error occurred while updating the record." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);

                if (!deleted)
                    return NotFound(new { Message = $"Product classification with ID {id} not found." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product classification with ID: {id}");
                return StatusCode(500, new { Message = "An error occurred while deleting the record." });
            }
        }

        [HttpPatch("{id}/soft-delete")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var deleted = await _service.SoftDeleteAsync(id);

                if (!deleted)
                    return NotFound(new { Message = $"Product classification with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error soft deleting product classification with ID: {id}");
                return StatusCode(500, new { Message = "An error occurred while deleting the record." });
            }
        }

        [HttpPatch("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var toggled = await _service.ToggleActiveStatusAsync(id);

                if (!toggled)
                    return NotFound(new { Message = $"Product classification with ID {id} not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling active status for product classification with ID: {id}");
                return StatusCode(500, new { Message = "An error occurred while updating the record." });
            }
        }

        [HttpGet("exists/product-code/{productCode}")]
        public async Task<IActionResult> CheckProductCodeExists(string productCode, [FromQuery] int? excludeId = null)
        {
            try
            {
                var exists = await _service.ExistsByProductCodeAsync(productCode, excludeId);
                return Ok(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking product code existence: {productCode}");
                return StatusCode(500, new { Message = "An error occurred while checking product code." });
            }
        }
    }
}