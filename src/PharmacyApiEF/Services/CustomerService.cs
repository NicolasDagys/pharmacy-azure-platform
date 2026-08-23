using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PharmacyApiEF.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly PharmacyContext _context;

        public CustomerService(PharmacyContext context)
        {
            _context = context;
        }

        public string? ValidateCustomer(CustomerDto dto)
        {
            if (dto == null)
                return "Customer is required.";

            if (string.IsNullOrWhiteSpace(dto.IdCus))
                return "Customer ID is required.";

            if (!Regex.IsMatch(
                    dto.IdCus,
                    @"^([1-6]\d{6}-\d|[1-9]\d{5}-\d)$"))
            {
                return "Invalid customer ID format.";
            }

            if (string.IsNullOrWhiteSpace(dto.NameCus))
                return "Customer name is required.";

            if (dto.NameCus.Length < 3)
                return "Customer name must contain at least 3 characters.";

            if (string.IsNullOrWhiteSpace(dto.MailCus))
                return "Email is required.";

            try
            {
                var email =
                    new System.Net.Mail.MailAddress(dto.MailCus);
            }
            catch
            {
                return "Invalid email format.";
            }

            if (!string.IsNullOrWhiteSpace(dto.PhoneCus))
            {
                if (!Regex.IsMatch(dto.PhoneCus, @"^\d+$"))
                {
                    return "Phone must contain only digits.";
                }
            }

            return null;
        }
    }
}