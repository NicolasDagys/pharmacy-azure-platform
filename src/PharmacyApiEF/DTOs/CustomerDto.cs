using PharmacyApiEF.Models;

namespace PharmacyApiEF.DTOs
{
    public class CustomerDto
    {
            public string IdCus { get; set; } = null!;

            public string NameCus { get; set; } = null!;

            public string MailCus { get; set; } = null!;

            public string? PhoneCus { get; set; }
        }
}
