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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PharmacyApiEF.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            PharmacyContext context,
            IProductService productService,
            ILogger<ProductsController> logger)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
        }

        //GET    /api/products
        [HttpGet]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts([FromQuery] QueryParameters query)
        {

            //base query
            IQueryable<Product> products = _context.Products
        .AsNoTracking()
        .Where(p => p.Active)
        .ApplyProductSearch(query)
        .ApplyProductSorting(query)
        .ApplyPagination(query);

            var result = await products
                .Select(p => new ProductDto
                {
                    CodProd = p.CodProd,
                    NameProd = p.NameProd,
                    PriceProd = p.PriceProd,
                    ExpDateProd = p.ExpDateProd,
                    PresentationTypeProd = p.PresentationTypeProd,
                    SizeProd = p.SizeProd,
                    CodeCat = p.CodeCat,
                    StockQty = p.StockQty
                })
                .ToListAsync();

            return Ok(result);
        }

        //GET    /api/products/{codProd}
        [HttpGet("{codProd}")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<Product>> GetProduct(string codProd)
        {
            var p = await _context.Products.FindAsync(codProd);

            if (p == null || p.Active == !p.Active)
            {
                return NotFound();
            }    

            var dto = new ProductDto
            {
                CodProd = p.CodProd,
                NameProd = p.NameProd,
                PriceProd = p.PriceProd,
                ExpDateProd = p.ExpDateProd,
                PresentationTypeProd = p.PresentationTypeProd,
                SizeProd = p.SizeProd,
                CodeCat = p.CodeCat,
                StockQty = p.StockQty,
            };

            return Ok(dto);
        }

        //POST   /api/products
        [HttpPost]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<ActionResult<Product>> CreateProduct(ProductDto dto)
        {
            var validation = _productService.ValidateProduct(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }
            var existingProduct = await _context.Products.FirstOrDefaultAsync(p => p.CodProd == dto.CodProd);

            if (existingProduct != null)
            {
                if (existingProduct.Active)
                {
                    return Conflict("Product already exists.");
                }

                existingProduct.Active = true;
                existingProduct.NameProd = dto.NameProd;
                existingProduct.PriceProd = dto.PriceProd;
                existingProduct.ExpDateProd = dto.ExpDateProd;
                existingProduct.PresentationTypeProd = dto.PresentationTypeProd;
                existingProduct.SizeProd = dto.SizeProd;
                existingProduct.CodeCat = dto.CodeCat;
                existingProduct.StockQty = dto.StockQty;

                await _context.SaveChangesAsync();

                return Ok(existingProduct);
            }

            var p = new Product
            {
                CodProd = dto.CodProd,
                NameProd = dto.NameProd,
                PriceProd = dto.PriceProd,
                ExpDateProd = dto.ExpDateProd,
                PresentationTypeProd = dto.PresentationTypeProd,
                SizeProd = dto.SizeProd,
                CodeCat = dto.CodeCat,
                StockQty = dto.StockQty,
                Active = true
            };

            _context.Products.Add(p);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Product {CodProd} created.", dto.CodProd);

            return CreatedAtAction(nameof(GetProduct), new { codProd = p.CodProd }, p);

        }

        //PUT    /api/products/{codProd}
        [HttpPut("{codProd}")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> UpdateProduct(string codProd, ProductDto dto) 
        {
            var validation = _productService.ValidateProduct(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var existingProduct =await _context.Products.FindAsync(codProd);

            if (existingProduct == null)
            {
                return NotFound();
            }
            if (!existingProduct.Active)
            {
                return BadRequest("Inactive products cannot be modified.");
            }

            existingProduct.NameProd = dto.NameProd;
            existingProduct.PriceProd = dto.PriceProd;
            existingProduct.ExpDateProd = dto.ExpDateProd;
            existingProduct.PresentationTypeProd = dto.PresentationTypeProd;
            existingProduct.SizeProd = dto.SizeProd;
            existingProduct.CodeCat = dto.CodeCat;
            existingProduct.StockQty = dto.StockQty;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ProductExists(codProd)) 
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

        //DELETE /api/products/{codProd}
        [HttpDelete("{codProd}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(string codProd)
        {
            var p = await _context.Products.FindAsync(codProd); //deberia usar var en vez de tipo Product??

            if (p == null || !p.Active)
            {
                return NotFound();
            }
            

            bool hasInvoices = await _context.InvoiceLines.AnyAsync(i => i.CodProd == codProd);

            if (!hasInvoices)
            {
                _context.Products.Remove(p);
            }
            else
            {
                p.Active = false;
            }
    
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product {codProd} deleted.", codProd);

            return NoContent();
        }

        //GET /api/products/low-stock
        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<Product>>> GetProductsLowStock([FromQuery] QueryParameters query)
        {

            //base query
            IQueryable<Product> products = _context.Products
        .AsNoTracking()
        .Where(p => p.Active && p.StockQty < 10);

            if (string.IsNullOrWhiteSpace(query.SortBy))
            {
                products = products.OrderBy(p => p.StockQty);
                
            }
            else
            {
                products = products.ApplyProductSorting(query);
            }

            products = products
        .ApplyProductSearch(query)
        .ApplyPagination(query);

            var result = await products
                .Select(p => new ProductDto
                {
                    CodProd = p.CodProd,
                    NameProd = p.NameProd,
                    PriceProd = p.PriceProd,
                    ExpDateProd = p.ExpDateProd,
                    PresentationTypeProd = p.PresentationTypeProd,
                    SizeProd = p.SizeProd,
                    CodeCat = p.CodeCat,
                    StockQty = p.StockQty
                })
                .ToListAsync();

            _logger.LogInformation(
    "Low stock report generated. Products found: {Count}",
    result.Count);

            return Ok(result);
        }

        //GET /api/products/presentation-types
        [HttpGet("presentation-types")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<string>>> GetPresentationTypes()
        {
            var presentationTypes = await _context.Products

                .AsNoTracking()

                .Select(p => p.PresentationTypeProd)

                .Distinct()

                .OrderBy(x => x)

                .ToListAsync();

            return Ok(presentationTypes);
        }
        //this one is used to check all active products (lookup) to populate the ddl on the invoice creation. the other endponts has pagination, searching and sorting
        //GET   /api/products/lookup
        [HttpGet("lookup")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<ProductLookupDto>>> GetProductLookup()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Where(p => p.Active)
                .OrderBy(p => p.NameProd)
                .Select(p => new ProductLookupDto
                {
                    CodProd = p.CodProd,
                    NameProd = p.NameProd,
                    PriceProd = p.PriceProd
                })
                .ToListAsync();

            return Ok(products);
        }

        private async Task<bool> ProductExists(string codProd)
        {
            return await _context.Products
                .AnyAsync(p => p.CodProd == codProd);
        }
    }
}
