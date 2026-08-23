using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Controllers
{
    [ApiController]
    [Route("api/ai")]
    [Authorize(Roles = "Admin,Sales,Pharmacist")]
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

            var response = await _aiInventoryService.AskAsync(
                request.Question);

            return Ok(response);
        }
    }
}
