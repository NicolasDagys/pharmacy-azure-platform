using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface ICategoryService
    {
        string? ValidateCategory(CategoryDto dto);
    }
}
