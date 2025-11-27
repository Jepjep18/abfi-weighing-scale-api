using abfi_weighing_scale_api.Models.Dtos;
using abfi_weighing_scale_api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto dto)
        {
            var user = await _service.CreateUserAsync(dto);
            return Ok(user);
        }
    }

}
