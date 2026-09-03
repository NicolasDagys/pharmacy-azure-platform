using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services.Interfaces;
using System.Text.RegularExpressions;

namespace PharmacyApiEF.Services
{
    public class ProductService : IProductService
    {

        private static readonly string[] ValidPresentationTypes =
        {
            "Tablets",
            "Syrups",
            "Creams",
            "Inhalers"
        };

        public string? ValidateProduct(ProductDto dto)
        {
            if (dto == null)
                return "Product is required.";

            // CodProd

            if (string.IsNullOrWhiteSpace(dto.CodProd))
                return "Product code is required.";

            if (!Regex.IsMatch(dto.CodProd, @"^[A-Z]{3}[0-9]{7}$"))
                return "Product code must have format AAA9999999.";

            // NameProd

            if (string.IsNullOrWhiteSpace(dto.NameProd))
                return "Product name is required.";

            if (dto.NameProd.Trim().Length < 3)
                return "Product name must contain at least 3 characters.";

            // PriceProd

            if (dto.PriceProd <= 0)
                return "Product price must be greater than zero.";

            // ExpDateProd

            if (dto.ExpDateProd <= DateOnly.FromDateTime(DateTime.Today))
                return "Expiration date must be in the future.";

            // PresentationTypeProd

            if (string.IsNullOrWhiteSpace(dto.PresentationTypeProd))
                return "Presentation type is required.";

            if (!ValidPresentationTypes.Contains(dto.PresentationTypeProd))
            {
                return "Presentation type must be Tablets, Syrups, Creams or Inhalers.";
            }

            // SizeProd

            if (dto.SizeProd <= 0)
                return "Size must be greater than zero.";

            // StockQty

            if (dto.StockQty < 0)
                return "Stock cannot be negative.";

            // CodeCat

            if (string.IsNullOrWhiteSpace(dto.CodeCat))
                return "Category code is required.";

            if (!Regex.IsMatch(dto.CodeCat, @"^[A-Z]{3}[0-9]{3}$"))
                return "Category code must have format AAA999.";

            return null;
        }
    }
}