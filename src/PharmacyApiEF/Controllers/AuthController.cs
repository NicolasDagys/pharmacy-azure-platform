using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Auth;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        //POST   /api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            if (dto == null)
                return BadRequest();

            string? token = await _authService.Login(dto);

            if (token == null)
            {
                _logger.LogWarning(
                    "Invalid login attempt for user {UserEmp}.",
                    dto.UserEmp);

                return Unauthorized("Invalid credentials.");
            }

            _logger.LogInformation(
                "User {UserEmp} logged in successfully.",
                dto.UserEmp);

            return Ok(new
            {
                Token = token
            });
        }

        //GET /api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> Me()
        {
            Employee? employee =
                await _authService.GetCurrentUser(User);

            if (employee == null)
                return NotFound();

            return Ok(new
            {
                employee.UserEmp,
                employee.NameEmp,
                employee.RoleEmp
            });
        }
    }
}

