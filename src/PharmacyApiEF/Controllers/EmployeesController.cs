using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.DTOs.Common;
using PharmacyApiEF.Extensions;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services;
using PharmacyApiEF.Services.Interfaces;

namespace PharmacyApiEF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(PharmacyContext context, IEmployeeService employeeService, ILogger<EmployeesController> logger)
        {
            _context = context;
            _employeeService = employeeService;
            _logger = logger;
        }

        //GET     /api/employees
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EmployeeDto>>> GetEmployees([FromQuery] QueryParameters query)
        {
            IQueryable<Employee> employees = _context.Employees
                .AsNoTracking()
                .ApplyEmployeeSearch(query)
                .ApplyEmployeeSorting(query)
                .ApplyPagination(query);

            var result = await employees
                .Select(e => new EmployeeDto
                {
                    UserEmp = e.UserEmp,
                    NameEmp = e.NameEmp,
                    RoleEmp = e.RoleEmp
                })
                .ToListAsync();

            return Ok(result);
        }

        //GET     /api/employees/{userEmp}
        [HttpGet("{userEmp}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> GetEmployee(string userEmp)
        {
            var employee = await _context.Employees.FindAsync(userEmp);

            if (employee == null)
            {
                return NotFound();
            }

            var dto = new EmployeeDto
            {
                UserEmp = employee.UserEmp,
                NameEmp = employee.NameEmp,
                RoleEmp = employee.RoleEmp
            };

            return Ok(dto);
        }

        //POST    /api/employees
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EmployeeDto>> CreateEmployee(EmployeeCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest();
            }

            var validation = _employeeService.ValidateEmployee(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserEmp == dto.UserEmp);

            if (existingEmployee != null)
            {
                return Conflict("Employee already exists.");
            }

            var employee = new Employee
            {
                UserEmp = dto.UserEmp,
                NameEmp = dto.NameEmp,
                PassHashEmp = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleEmp = dto.RoleEmp
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Employee {UserEmp} created.",
                dto.UserEmp);

            var response = new EmployeeDto
            {
                UserEmp = employee.UserEmp,
                NameEmp = employee.NameEmp,
                RoleEmp = employee.RoleEmp
            };

            return CreatedAtAction(
                nameof(GetEmployee),
                new { userEmp = employee.UserEmp },
                response);
        }

        //PUT     /api/employees/{userEmp} change name
        [HttpPut("{userEmp}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(string userEmp,EmployeeUpdateDto dto)
        {

            var validation = _employeeService.ValidateEmployee(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var employee = await _context.Employees.FindAsync(userEmp);

            if (employee == null)
            {
                return NotFound();
            }

            employee.NameEmp = dto.NameEmp;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EmployeeExists(userEmp))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        //PUT   /api/employees/change-password
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(EmployeeChangePasswordDto dto)
        {
            var validation = _employeeService.ValidateChangePassword(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var username = User.Identity!.Name!;

            var employee = await _context.Employees.FindAsync(username);

            if (employee == null)
            {
                return NotFound();
            }

            if (!BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                employee.PassHashEmp))
            {
                return BadRequest("Current password is incorrect.");
            }

            employee.PassHashEmp =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Employee {username} changed password.",
                username);

            return NoContent();
        }

        //PUT   /api/employees/{userEmp}/role  change only the role
        [HttpPut("{userEmp}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(string userEmp, EmployeeChangeRole dto)
        {
            var validation = _employeeService.ValidateChangeRole(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var employee = await _context.Employees.FindAsync(userEmp);

            if (employee == null)
            {
                return NotFound();
            }

            employee.RoleEmp = dto.RoleEmp;

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Employee {userEmp} role changed to {RoleEmp}.",
                userEmp,
                dto.RoleEmp);

            return NoContent();
        }

        //DELETE   /api/employees/{userEmp}
        [HttpDelete("{userEmp}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee(string userEmp)
        {
            string currentUser = User.Identity!.Name!;

            if (currentUser.Equals(userEmp, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("You cannot delete your own account.");
            }

            var employee = await _context.Employees.FindAsync(userEmp);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Employee {userEmp} deleted.",
                userEmp);

            return NoContent();
        }

        private async Task<bool> EmployeeExists(string userEmp)
        {
            return await _context.Employees
                .AnyAsync(e => e.UserEmp == userEmp);
        }
    }
}
