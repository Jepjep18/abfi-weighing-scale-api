using abfi_weighing_scale_api.Services.Interfaces;
using abfi_weighing_scale_api.Services.ProductClassifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace abfi_weighing_scale_api.Controllers.ProductClassification
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductClassificationController : ControllerBase
    {
        private readonly IProductClassificationService _service;

        public ProductClassificationController(IProductClassificationService service)
        {
            _service = service;
        }

        /// <summary>
        /// Upload a CSV file with product classification master data.
        /// CSV must have headers: PRODUCT CODE, INDIVIDUAL WEIGHT RANGE, TOTAL WEIGHT RANGE PER CRATE, NO. HEADS PER GALANTINA, CRATES WEIGHT
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] ProductClassificationFileUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest("No file uploaded.");

            var result = await _service.UploadAsync(dto.File);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                Message = result.Message,
                RecordsInserted = result.Count
            });
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();

            return Ok(result);
        }


    }
}
