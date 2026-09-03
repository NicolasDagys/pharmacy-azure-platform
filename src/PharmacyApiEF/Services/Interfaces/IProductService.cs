using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface IProductService
    {
        string? ValidateProduct(ProductDto dto);
    }
}
