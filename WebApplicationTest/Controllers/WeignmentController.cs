using Microsoft.AspNetCore.Mvc;
using WebApplicationTest.Service;

namespace WebApplicationTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightController : ControllerBase
    {
        private readonly WeightService _weightService;

        public WeightController(WeightService weightService)
        {
            _weightService = weightService;
        }

        [HttpGet("read-weight")]
        public IActionResult ReadWeight()
        {
            var result = _weightService.ReadWeight();

            return Ok(result); 
        }
    }
}