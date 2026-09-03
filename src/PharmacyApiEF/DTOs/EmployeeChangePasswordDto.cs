namespace PharmacyApiEF.DTOs
{
    public class EmployeeChangePasswordDto
    {
        public string CurrentPassword { get; set; } = null!;

        public string NewPassword { get; set; } = null!;
    }
}
