using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }


        //GET    /api/dashboard
        [HttpGet]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var dashboard =
                await _dashboardService.GetDashboard();

            return Ok(dashboard);
        }
    }
}
