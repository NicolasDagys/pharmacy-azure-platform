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
    public class CategoriesController : ControllerBase
    {
        private readonly PharmacyContext _context;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;
        public CategoriesController(PharmacyContext context, ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _context = context;
            _categoryService = categoryService;
            _logger = logger;
        }

        //GET    /api/categories
        [HttpGet]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories([FromQuery] QueryParameters query)
        {
            IQueryable<Category> categories = _context.Categories
                .AsNoTracking()
                .Where(c => c.Active)
                .ApplyCategorySearch(query)
                .ApplyCategorySorting(query)
                .ApplyPagination(query);

                var result = await categories
                .Select(c => new CategoryDto 
                {
                    CodeCat = c.CodeCat,
                    NameCat = c.NameCat,
     
                })
                .ToListAsync();

            return Ok(result);
        }

        //GET    /api/categories/{codeCat}
        [HttpGet("{codeCat}")]
        [Authorize(Roles = "Admin,Sales,Pharmacist")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string codeCat)
        {
            var c = await _context.Categories.FindAsync(codeCat);

            if (c == null || c.Active ==false)
            {
                return NotFound();
            }
            var dto = new CategoryDto
            {
                CodeCat = c.CodeCat,
                NameCat = c.NameCat,
            };

            return Ok(dto);
        }

        //POST   /api/categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CategoryDto dto)
        {
            //Validations
            var validation = _categoryService.ValidateCategory(dto);

            if (validation != null)
            {
                return BadRequest(validation);
            }

            var existingCategory = await _context.Categories.FirstOrDefaultAsync(x => x.CodeCat == dto.CodeCat); 

            if(existingCategory!=null)
            {
                if(existingCategory.Active)
                {
                    return Conflict("Category already exists.");
                }

                existingCategory.Active = true;
                existingCategory.NameCat = dto.NameCat;

                await _context.SaveChangesAsync();

                return Ok(dto);
            }
            var category = new Category
            {
                CodeCat = dto.CodeCat,
                NameCat = dto.NameCat,
                Active = true
            };

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {CodeCat} created.", dto.CodeCat);

            return CreatedAtAction(
                nameof(GetCategory),
                new { codeCat = category.CodeCat },
                dto);


        }

        //PUT    /api/categories/{codeCat}
        [HttpPut("{codeCat}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(string codeCat, CategoryDto dto) 
        {
            if (codeCat != dto.CodeCat)
            {
                return BadRequest();
            }
            if (dto.NameCat.Length < 3)
            {
                return BadRequest(
                    "Category name must contain at least 3 characters.");
            }

            var existingCategory = await _context.Categories.FindAsync(codeCat);

            if (existingCategory == null)
            {
                return NotFound();
            }
            if (!existingCategory.Active)
            {
                return BadRequest("Inactive categories cannot be modified.");
            }
            existingCategory.NameCat = dto.NameCat;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (await CategoryExists(codeCat)) 
                {
                    return NotFound();
                }
            }
            return NoContent();
        }

        //DELETE /api/products/{codProd}
        [HttpDelete("{codeCat}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(string codeCat) 
        {
            var c = await _context.Categories.FindAsync(codeCat); 

            if (c == null || !c.Active)
            {
                return NotFound();
            }
            if (c.NameCat.Length < 3)
            {
                return BadRequest(
                    "Category name must contain at least 3 characters.");
            }
            bool hasActiveProducts = await _context.Products.AnyAsync(p => p.CodeCat == codeCat && p.Active);

            if (!hasActiveProducts)
            {
                _context.Categories.Remove(c);
            }
            else
            {
                c.Active = false;
            }


            await _context.SaveChangesAsync();

            _logger.LogInformation("Category {codeCat} deleted.", codeCat);
            return NoContent();
        }

        private async Task<bool> CategoryExists(string codeCat)
        {
            return await _context.Categories
                .AnyAsync(c => c.CodeCat == codeCat);
        }
    }
}

