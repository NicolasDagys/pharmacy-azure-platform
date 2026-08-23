using Microsoft.EntityFrameworkCore;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PharmacyApiEF.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly PharmacyContext _context; 

        public CategoryService(PharmacyContext context)
        {
            _context = context;
        }

        public string? ValidateCategory(CategoryDto dto)
        {
            if (dto == null)
                return "Category is required.";

            if (string.IsNullOrWhiteSpace(dto.CodeCat))
                return "Category code is required.";

            if (!Regex.IsMatch(dto.CodeCat, @"^[A-Z]{3}[0-9]{3}$"))
                return "Category code must have format AAA999.";

            if (string.IsNullOrWhiteSpace(dto.NameCat))
                return "Category name is required.";

            if (dto.NameCat.Length < 3)
                return "Category name must contain at least 3 characters.";

            return null;
        }
    }
}
