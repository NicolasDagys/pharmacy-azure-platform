using Microsoft.IdentityModel.Tokens;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace PharmacyApiEF.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly PharmacyContext _context;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            var options =
                new DbContextOptionsBuilder<PharmacyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PharmacyContext(options);

            IConfiguration configuration =
                new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "THIS_IS_A_DEMO_SECRET_KEY_FOR_PORTFOLIO_2026",
                    ["Jwt:Issuer"] = "PharmacyApi",
                    ["Jwt:Audience"] = "PharmacyApiUsers",
                    ["Jwt:ExpireMinutes"] = "60"
                })
                .Build();

            _service = new AuthService(_context, configuration);
        }

        //----------------------------------------------------
        // Login
        //----------------------------------------------------

        [Fact]
        public async Task Login_UserNotFound_ReturnsNull()
        {
            var dto = new LoginDto
            {
                UserEmp = "Admin",
                Password = "1234"
            };

            string? token = await _service.Login(dto);

            Assert.Null(token);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsNull()
        {
            _context.Employees.Add(new Employee
            {
                UserEmp = "Admin",
                NameEmp = "Administrator",
                RoleEmp = "Admin",
                PassHashEmp =
                    BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
            });

            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                UserEmp = "Admin",
                Password = "WrongPassword"
            };

            string? token = await _service.Login(dto);

            Assert.Null(token);
        }

        [Fact]
        public async Task Login_InvalidHash_ReturnsNull()
        {
            _context.Employees.Add(new Employee
            {
                UserEmp = "Admin",
                NameEmp = "Administrator",
                RoleEmp = "Admin",
                PassHashEmp = "INVALID_HASH"
            });

            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                UserEmp = "Admin",
                Password = "1234"
            };

            string? token = await _service.Login(dto);

            Assert.Null(token);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            _context.Employees.Add(new Employee
            {
                UserEmp = "Admin",
                NameEmp = "Administrator",
                RoleEmp = "Admin",
                PassHashEmp =
                    BCrypt.Net.BCrypt.HashPassword("Password123")
            });

            await _context.SaveChangesAsync();

            var dto = new LoginDto
            {
                UserEmp = "Admin",
                Password = "Password123"
            };

            string? token = await _service.Login(dto);

            Assert.NotNull(token);

            Assert.NotEmpty(token);
        }

        //----------------------------------------------------
        // GetCurrentUser
        //----------------------------------------------------

        [Fact]
        public async Task GetCurrentUser_NoIdentity_ReturnsNull()
        {
            var principal = new ClaimsPrincipal();

            Employee? employee =
                await _service.GetCurrentUser(principal);

            Assert.Null(employee);
        }

        [Fact]
        public async Task GetCurrentUser_UserExists_ReturnsEmployee()
        {
            _context.Employees.Add(new Employee
            {
                UserEmp = "Admin",
                NameEmp = "Administrator",
                RoleEmp = "Admin",
                PassHashEmp =
                    BCrypt.Net.BCrypt.HashPassword("Password123")
            });

            await _context.SaveChangesAsync();

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, "Admin")
            ],
            authenticationType: "Test");

            var principal = new ClaimsPrincipal(identity);

            Employee? employee =
                await _service.GetCurrentUser(principal);

            Assert.NotNull(employee);

            Assert.Equal("Admin", employee!.UserEmp);

            Assert.Equal("Administrator", employee.NameEmp);

            Assert.Equal("Admin", employee.RoleEmp);
        }
    }
}
