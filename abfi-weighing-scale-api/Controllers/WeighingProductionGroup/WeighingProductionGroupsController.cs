using abfi_weighing_scale_api.Controllers.Production;
using abfi_weighing_scale_api.Models.DTOs.WeighingProductionGroup;
using abfi_weighing_scale_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.WeighingProductionGroup
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeighingProductionGroupsController : ControllerBase
    {
        private readonly IWeighingProductionGroupService _weighingProductionGroupService;

        public WeighingProductionGroupsController(IWeighingProductionGroupService weighingProductionGroupService)
        {
            _weighingProductionGroupService = weighingProductionGroupService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateWeighingProductionGroupDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdGroup = await _weighingProductionGroupService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdGroup.Id }, createdGroup);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while creating the weighing production group." });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _weighingProductionGroupService.GetByIdAsync(id);

            if (group == null)
            {
                return NotFound(new { message = $"WeighingProductionGroup with ID {id} not found." });
            }

            return Ok(group);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ProductionRequestDto request)
        {
            var groups = await _weighingProductionGroupService.GetAllAsync(request);
            return Ok(groups);
        }

    }
}
