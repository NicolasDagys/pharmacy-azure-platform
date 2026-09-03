using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.DTOs.Common;
using PharmacyApiEF.Extensions;
using PharmacyApiEF.Middleware;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using System.Data;

namespace PharmacyApiEF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(
            PharmacyContext context,
            IInvoiceService invoiceService,
            ILogger<InvoicesController> logger)
        {
            _context = context;
            _invoiceService = invoiceService;
            _logger = logger;
        }

        //GET    /api/invoices  --view all
        [HttpGet]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<InvoiceDto>>> GetInvoices(
    [FromQuery] QueryParameters query)
        {
            IQueryable<Invoice> invoices =
                _context.Invoices
                    .AsNoTracking()
                    .Include(i => i.Assignments)
                        .ThenInclude(a => a.NumbStaNavigation)
                    .ApplyInvoiceSearch(query)
                    .ApplyInvoiceSorting(query)
                    .ApplyPagination(query);

            var result =
                await invoices
                    .Select(i => new InvoiceDto
                    {
                        NumbInv = i.NumbInv,
                        DateInv = i.DateInv,
                        ShipmentAddressInv = i.ShipmentAddressInv,
                        TotalInv = i.TotalInv,
                        IdCus = i.IdCus,
                        Status =
                            i.Assignments
                                .OrderByDescending(a => a.DateTimeStatus)
                                .Select(a => a.NumbStaNavigation.NameSta)
                                .FirstOrDefault() ?? ""
                    })
                    .ToListAsync();

            return Ok(result);
        }

        //GET    /api/invoices/{numbInv}  --details
        [HttpGet("{numbInv}")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<InvoiceDetailsDto>> GetInvoice(int numbInv)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.IdCusNavigation)
                .Include(i => i.InvoiceLines)
                    .ThenInclude(l => l.CodProdNavigation)
                .Include(i => i.Assignments)
                    .ThenInclude(a => a.NumbStaNavigation)
                .FirstOrDefaultAsync(i => i.NumbInv == numbInv);

            if (invoice == null)
                return NotFound();

            string status =
                invoice.Assignments
                    .OrderByDescending(a => a.DateTimeStatus)
                    .FirstOrDefault()?
                    .NumbStaNavigation
                    .NameSta
                ?? "";

            var dto = new InvoiceDetailsDto
            {
                NumbInv = invoice.NumbInv,

                DateInv = invoice.DateInv,

                ShipmentAddressInv = invoice.ShipmentAddressInv,

                TotalInv = invoice.TotalInv,

                IdCus = invoice.IdCus,

                CustomerName = invoice.IdCusNavigation.NameCus,

                Status = status,

                InvoiceLines = invoice.InvoiceLines
                    .Select(l => new InvoiceLineDetailDto
                    {
                        CodProd = l.CodProd,

                        ProductName = l.CodProdNavigation.NameProd,

                        UnitPrice = l.UnitPrice,

                        Quantity = l.Cant,

                        SubTotal = l.UnitPrice * l.Cant
                    })
                    .ToList()
            };

            return Ok(dto);
        }

        //POST   /api/invoices    --create
        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        [HttpPost]
        [Authorize(Roles = "Admin,Sales")]
        public async Task<ActionResult> CreateInvoice(
    InvoiceCreateDto dto)
        {
            var validationError =
                _invoiceService.ValidateInvoice(dto);

            if (validationError != null)
            {
                throw new ValidationException(
                    validationError);
            }

            if (!await _invoiceService.CustomerExists(dto.IdCus))
            {
                throw new NotFoundException(
                    "Customer does not exist.");
            }

            if (!await _invoiceService.ProductsExist(dto.InvoiceLines))
            {
                throw new NotFoundException(
                    "One or more products do not exist.");
            }

            if (!await _invoiceService.HasEnoughStock(dto.InvoiceLines))
            {
                throw new ValidationException(
                    "Insufficient stock.");
            }

            using SqlConnection connection =
                new SqlConnection(
                    _context.Database.GetConnectionString());

            await connection.OpenAsync();

            using SqlCommand command =
                new SqlCommand(
                    "sp_CreateInvoice",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@IdCus",
                dto.IdCus);

            command.Parameters.AddWithValue(
                "@ShipmentAddressInv",
                dto.ShipmentAddressInv);

            DataTable linesTable =
                new DataTable();

            linesTable.Columns.Add(
                "CodProd",
                typeof(string));

            linesTable.Columns.Add(
                "Cant",
                typeof(int));

            foreach (var line in dto.InvoiceLines)
            {
                linesTable.Rows.Add(
                    line.CodProd,
                    line.Quantity);
            }

            SqlParameter tvp =
                command.Parameters.AddWithValue(
                    "@Lines",
                    linesTable);

            tvp.SqlDbType =
                SqlDbType.Structured;

            tvp.TypeName =
                "dbo.InvoiceLineType";

            SqlParameter returnValue =
                command.Parameters.Add(
                    "@ReturnValue",
                    SqlDbType.Int);

            returnValue.Direction =
                ParameterDirection.ReturnValue;

            object? invoiceNumber =
                await command.ExecuteScalarAsync();

            int result =
                (int)returnValue.Value;

            switch (result)
            {
                case -1:

                    throw new NotFoundException(
                        "Customer does not exist.");

                case -2:

                    throw new NotFoundException(
                        "One or more products do not exist.");

                case -3:

                    throw new ValidationException(
                        "Insufficient stock.");

                case 0:

                    int createdInvoice =
                        Convert.ToInt32(
                            invoiceNumber);

                    _logger.LogInformation(
                        "Invoice {InvoiceNumber} created for customer {CustomerId}.",
                        createdInvoice,
                        dto.IdCus);

                    return CreatedAtAction(
                        nameof(GetInvoice),
                        new
                        {
                            numbInv = createdInvoice
                        },
                        new
                        {
                            InvoiceNumber = createdInvoice
                        });

                default:

                    throw new Exception(
                        "Unexpected result returned by stored procedure.");
            }
        }

        //PUT /api/invoices/{numbInv}/status   --change status
        [HttpPut("{numbInv}/status")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult> UpdateInvoiceStatus(int numbInv,StatusChangeDto dto)
        {
            if (!await _invoiceService.InvoiceExists(numbInv))
            {
                return NotFound();
            }

            var status =
                await _context.OrderStatuses
                    .FirstOrDefaultAsync(
                        s => s.NumbSta == dto.NumbSta);

            if (status == null)
            {
                return BadRequest(
                    "Invalid status.");
            }

            if (!await _invoiceService
                .IsValidStatusTransition(
                    numbInv,
                    dto.NumbSta))
            {
                return BadRequest(
                    "Invalid status transition.");
            }

            var assignment = new Assignment
            {
                NumbInv = numbInv,
                NumbSta = dto.NumbSta,
                DateTimeStatus = DateTime.Now
            };

            _context.Assignments.Add(assignment);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Invoice {numbInv} changed to status {NameSta}.", numbInv,status.NameSta);

            return NoContent();
        }
    }
}
