using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize(Roles = "Admin,Sales,Pharmacist")]
    [EnableRateLimiting("ai")]
    public class AiInventoryController : ControllerBase
    {
        private readonly IAiInventoryService _aiInventoryService;

        public AiInventoryController(
            IAiInventoryService aiInventoryService)
        {
            _aiInventoryService = aiInventoryService;
        }

        [HttpPost("inventory/chat")]
        public async Task<ActionResult<AiInventoryChatResponseDto>> Ask(
            [FromBody] AiInventoryChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Question is required.");
            }

            if (request.Question.Length > 500)
            {
                return BadRequest("Question must be 500 characters or fewer.");
            }

            var response = await _aiInventoryService.AskAsync(
                request.Question);

            return Ok(response);
        }
    }
}
