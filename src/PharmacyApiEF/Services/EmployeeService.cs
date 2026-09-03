using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using PharmacyApiEF.Utilities;

namespace PharmacyApiEF.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly PharmacyContext _context;

        public EmployeeService(PharmacyContext context)
        {
            _context = context;
        }
        public string? ValidateEmployee(EmployeeCreateDto dto)
        {
            if (dto == null)
                return "Employee is required.";

            var validation = ValidateCommonForCreate(
                dto.UserEmp,
                dto.NameEmp,
                dto.RoleEmp);

            if (validation != null)
                return validation;

            return PasswordPolicy.Validate(dto.Password);
        }

        public string? ValidateEmployee(EmployeeUpdateDto dto)
        {
            if (dto == null)
                return "Employee is required.";

            return ValidateEmployeeName(dto.NameEmp);
        }

        public string? ValidateChangeRole(EmployeeChangeRole dto)
        {
            if (dto == null)
                return "Employee is required.";

            return ValidateRole(dto.RoleEmp);
        }

        public string? ValidateChangePassword(EmployeeChangePasswordDto dto)
        {
            if (dto == null)
                return "Request is required.";

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                return "Current password is required.";

            if (dto.CurrentPassword == dto.NewPassword)
                return "The new password must be different from the current password.";

            return PasswordPolicy.Validate(dto.NewPassword);
        }

        private string? ValidateCommonForCreate(
            string user,
            string name,
            string role)
        {
            if (string.IsNullOrWhiteSpace(user))
                return "Username is required.";

            if (user.Trim().Length < 3)
                return "Username must contain at least 3 characters.";

            if (user.Length > 50)
                return "Username cannot exceed 50 characters.";

            var nameValidation = ValidateEmployeeName(name);

            if (nameValidation != null)
                return nameValidation;

            return ValidateRole(role);
        }

        private string? ValidateEmployeeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Employee name is required.";

            if (name.Trim().Length < 3)
                return "Employee name must contain at least 3 characters.";

            if (name.Length > 50)
                return "Employee name cannot exceed 50 characters.";

            return null;
        }

        private string? ValidateRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return "Role is required.";

            string[] validRoles =
            {
        "Admin",
        "Pharmacist",
        "Sales"
    };

            if (!validRoles.Contains(role))
                return "Invalid role.";

            return null;
        }
    }

}