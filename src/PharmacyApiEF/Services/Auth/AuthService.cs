using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PharmacyApiEF.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly PharmacyContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(PharmacyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<string?> Login(LoginDto dto)
        {
            Employee? employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserEmp == dto.UserEmp);

            if (employee == null)
                return null;

            bool valid =
                BCrypt.Net.BCrypt.Verify(dto.Password,employee.PassHashEmp);

            if (!valid)
                return null;

            return GenerateJwtToken(employee);
        }

        private string GenerateJwtToken(Employee employee)
        {

        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            /*var claims = new[]
            {
        new Claim(
            ClaimTypes.Name,
            employee.UserEmp),

        new Claim(
            ClaimTypes.Role,
            employee.RoleEmp),

        new Claim(
            "FullName",
            employee.NameEmp)
    };*/

            var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, employee.UserEmp),

    new Claim(
        ClaimTypes.Role,
        employee.RoleEmp
    )
};

            var credentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(
                        Convert.ToDouble(
                            _configuration["Jwt:ExpireMinutes"])),
                    signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public async Task<Employee?> GetCurrentUser(ClaimsPrincipal user)
        {
            string? username = user.Identity?.Name;

            if (username == null)
                return null;

            return await _context.Employees.FirstOrDefaultAsync(e => e.UserEmp == username);
        }

    }
}
