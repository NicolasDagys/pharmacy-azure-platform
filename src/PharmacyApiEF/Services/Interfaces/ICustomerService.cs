using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface ICustomerService
    {
        string? ValidateCustomer(CustomerDto dto);
    }
}
