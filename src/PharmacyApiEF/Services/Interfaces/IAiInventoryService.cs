using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface IAiInventoryService
    {
        Task<AiInventoryChatResponseDto> AskAsync(string question);
    }
}
