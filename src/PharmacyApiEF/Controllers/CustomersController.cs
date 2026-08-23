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
    public class CustomersController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;
        public CustomersController(PharmacyContext context, ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _context = context;
            _customerService = customerService;
            _logger = logger;
        }

        //GET    /api/customers
        [HttpGet]
        [Authorize(Roles = "Admin,Sales")]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomers([FromQuery] QueryParameters query)
        {

            IQueryable<Customer> customers = _context.Customers
         .AsNoTracking()
        .ApplyCustomerSearch(query)
        .ApplyCustomerSorting(query)
        .ApplyPagination(query);

            var result = await customers
                .Select(c => new CustomerDto
                {
                    IdCus = c.IdCus,
                    NameCus = c.NameCus,
                    MailCus = c.MailCus,
                    PhoneCus = c.PhoneCus,
                })
                .ToListAsync();

            return Ok(result);
        }

        //GET    /api/customers/{idCus}
        [HttpGet("{idCus}")]
        [Authorize(Roles = "Admin,Sales")]
        public async Task<ActionResult<CustomerDto>> GetCustomer(string idCus)
        {
            //falta separar y agregar validacionesa la clase de servicio
            var c = await _context.Customers.FindAsync(idCus);

            if (c == null)
            {
                return NotFound();
            }

            var dto = new CustomerDto
            {
                IdCus = c.IdCus,
                NameCus = c.NameCus,
                MailCus= c.MailCus,
                PhoneCus = c.PhoneCus,
            };

            return Ok(dto);
        }

        //POST   /api/customers
        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto dto)
        {
            var validation = _customerService.ValidateCustomer(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }
            if (dto == null)
            {
                return BadRequest();
            }
            
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.IdCus == dto.IdCus);

            if (existingCustomer != null)
            {
                return Conflict("Customer already exists.");
                
            }
            var c = new Customer
            {
                IdCus = dto.IdCus,
                NameCus = dto.NameCus,
                MailCus = dto.MailCus,
                PhoneCus = dto.PhoneCus,
            };
            _context.Customers.Add(c);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Customer {IdCus} created.", dto.IdCus);

            return CreatedAtAction(nameof(GetCustomer), new { idCus = c.IdCus }, dto);

        }

        //PUT    /api/customer/{idCus}
        [HttpPut("{idCus}")]
        [Authorize(Roles = "Admin,Sales")]
        public async Task<IActionResult> UpdateCustomer(string idCus, CustomerDto dto) 
        {
            if (idCus != dto.IdCus)
            {
                return BadRequest();
            }

            var validation = _customerService.ValidateCustomer(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var existingCustomer = await _context.Customers.FindAsync(idCus);

            if (existingCustomer == null)
            {
                return NotFound();
            }


            existingCustomer.NameCus = dto.NameCus;
            existingCustomer.MailCus = dto.MailCus;
            existingCustomer.PhoneCus = dto.PhoneCus;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(idCus))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //DELETE /api/customers/{idCus}
        [HttpDelete("{idCus}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(string idCus)
        {
            //falta separar y agregar validacionesa la clase de servicio
            var c = await _context.Customers.FindAsync(idCus);

            if (c == null)
            {
                return NotFound();
            }

            bool hasInvoices = await _context.Invoices.AnyAsync(i => i.IdCus == idCus);

            if (hasInvoices)
            {
                return BadRequest("Customer cannot be deleted because invoices exist.");

            }

            _context.Customers.Remove(c);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Customer {idCus} deleted.", idCus);

            return NoContent();
        }

        //this one is used to check all customers (lookup) to populate the ddl on the invoice creation. the other endponts has pagination, searching and sorting
        //GET   /api/customers/lookup
        [HttpGet("lookup")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<CustomerLookupDto>>> GetCustomerLookup()
        {
            var customers = await _context.Customers
                .AsNoTracking()
                .OrderBy(c => c.NameCus)
                .Select(c => new CustomerLookupDto
                {
                    IdCus = c.IdCus,
                    NameCus = c.NameCus
                })
                .ToListAsync();

            return Ok(customers);
        }

        private async Task<bool> CustomerExists(string idCus)
        {
            return await _context.Customers
                .AnyAsync(c => c.IdCus == idCus);
        }
    }

}
