using abfi_weighing_scale_api.Services.Farms;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace abfi_weighing_scale_api.Controllers.Farms
{
    [ApiController]
    [Route("api/[controller]")]
    public class FarmsController : ControllerBase
    {
        private readonly IFarmsService _farmsService;

        public FarmsController(IFarmsService farmsService)
        {
            _farmsService = farmsService;
        }

        // GET: api/farms
        [HttpGet]
        public async Task<IActionResult> GetAllFarms()
        {
            var farms = await _farmsService.GetAllFarmsAsync();
            return Ok(farms);
        }

        // GET: api/farms/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFarmById(int id)
        {
            var farm = await _farmsService.GetFarmByIdAsync(id);

            if (farm == null)
                return NotFound(new { Message = $"Farm with ID {id} not found" });

            return Ok(farm);
        }

        // POST: api/farms
        [HttpPost]
        public async Task<IActionResult> CreateFarm([FromBody] CreateFarmDto createFarmDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(createFarmDto.FarmName))
                return BadRequest(new { Message = "Farm name is required" });

            var farm = await _farmsService.CreateFarmAsync(createFarmDto);

            return CreatedAtAction(nameof(GetFarmById), new { id = farm.Id }, farm);
        }

        // PUT: api/farms/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, [FromBody] UpdateFarmDto updateFarmDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var farm = await _farmsService.UpdateFarmAsync(id, updateFarmDto);

            if (farm == null)
                return NotFound(new { Message = $"Farm with ID {id} not found" });

            return Ok(farm);
        }

        // DELETE: api/farms/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            var result = await _farmsService.DeleteFarmAsync(id);

            if (!result)
                return NotFound(new { Message = $"Farm with ID {id} not found" });

            return NoContent();
        }

        // PATCH: api/farms/{id}/toggle-status
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleFarmStatus(int id)
        {
            var result = await _farmsService.ToggleFarmStatusAsync(id);

            if (!result)
                return NotFound(new { Message = $"Farm with ID {id} not found" });

            return Ok(new { Message = $"Farm status updated successfully" });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFarms([FromForm] FileUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest("No file uploaded.");

            // Validate file type
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Only Excel files (.xlsx, .xls) are allowed.");

            var result = await _farmsService.UploadFarmsAsync(dto.File);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                Message = result.Message,
                RecordsInserted = result.Count
            });
        }
    }
}
