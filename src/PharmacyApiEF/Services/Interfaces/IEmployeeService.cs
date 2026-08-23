using PharmacyApiEF.DTOs;

namespace PharmacyApiEF.Services.Interfaces
{
    public interface IEmployeeService
    {
        string? ValidateEmployee(EmployeeCreateDto dto);

        string? ValidateEmployee(EmployeeUpdateDto dto);

        string? ValidateChangeRole(EmployeeChangeRole dto);

        string? ValidateChangePassword(EmployeeChangePasswordDto dto);
    }
}
