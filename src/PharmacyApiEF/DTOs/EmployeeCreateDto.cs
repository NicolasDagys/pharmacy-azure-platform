namespace PharmacyApiEF.DTOs
{
    public class EmployeeCreateDto
    {
        public string UserEmp { get; set; } = null!;

        public string NameEmp { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string RoleEmp { get; set; } = null!;
    }
}
