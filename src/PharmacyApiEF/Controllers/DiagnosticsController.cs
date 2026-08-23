using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PharmacyApiEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DiagnosticsController(
            IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Ok(new
            {
                Status = "OK",
                Environment = _environment.EnvironmentName,
                TimestampUtc = DateTime.UtcNow
            });
        }
    }
}
