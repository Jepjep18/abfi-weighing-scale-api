using abfi_weighing_scale_api.Services.WeighingDataProcessorService;
using abfi_weighing_scale_api.Services.WeighingService;
using Microsoft.AspNetCore.Mvc;

namespace abfi_weighing_scale_api.Controllers.TestDebuggerController
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public TestController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        [HttpGet("services")]
        public IActionResult CheckServices()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var weighingService = scope.ServiceProvider.GetService<IWeighingService>();
                var weighingProcessor = scope.ServiceProvider.GetService<IWeighingDataProcessor>();

                var services = new
                {
                    WeighingServiceRegistered = weighingService != null,
                    WeighingDataProcessorRegistered = weighingProcessor != null,
                    WeighingServiceType = weighingService?.GetType().Name,
                    WeighingDataProcessorType = weighingProcessor?.GetType().Name
                };

                return Ok(services);
            }
        }
    }
}
