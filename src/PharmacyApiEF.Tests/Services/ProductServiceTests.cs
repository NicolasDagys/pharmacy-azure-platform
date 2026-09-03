using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services;
using Xunit;

namespace PharmacyApiEF.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _service = new ProductService();
        }

        [Theory]
        [InlineData("", "Product code is required.")]
        [InlineData("ABC123", "Product code must have format AAA9999999.")]
        [InlineData("abc1234567", "Product code must have format AAA9999999.")]
        [InlineData("1234567890", "Product code must have format AAA9999999.")]
        public void ValidateProduct_InvalidProductCode_ReturnsError(
            string code,
            string expected)
        {
            var dto = ValidProduct();

            dto.CodProd = code;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ValidateProduct_EmptyName_ReturnsRequired()
        {
            var dto = ValidProduct();

            dto.NameProd = "";

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Product name is required.",
                result);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        public void ValidateProduct_NameTooShort_ReturnsError(string name)
        {
            var dto = ValidProduct();

            dto.NameProd = name;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Product name must contain at least 3 characters.",
                result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void ValidateProduct_InvalidPrice_ReturnsError(decimal price)
        {
            var dto = ValidProduct();

            dto.PriceProd = price;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Product price must be greater than zero.",
                result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void ValidateProduct_InvalidSize_ReturnsError(int size)
        {
            var dto = ValidProduct();

            dto.SizeProd = size;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Size must be greater than zero.",
                result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-10)]
        public void ValidateProduct_InvalidStock_ReturnsError(int stock)
        {
            var dto = ValidProduct();

            dto.StockQty = stock;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Stock cannot be negative.",
                result);
        }
        //in the case for presentation type, we will test for empty string and invalid values, and valid values
        [Fact]
        public void ValidateProduct_PresentationRequired_ReturnsError()
        {
            var dto = ValidProduct();

            dto.PresentationTypeProd = "";

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Presentation type is required.",
                result);
        }

        [Theory]
        [InlineData("Capsules")]
        [InlineData("Pills")]
        public void ValidateProduct_InvalidPresentation_ReturnsError(
    string presentation)
        {
            var dto = ValidProduct();

            dto.PresentationTypeProd = presentation;

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Presentation type must be Tablets, Syrups, Creams or Inhalers.",
                result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("AB1")]
        [InlineData("123456")]
        public void ValidateProduct_InvalidCategory_ReturnsError(
    string category)
        {
            var dto = ValidProduct();

            dto.CodeCat = category;

            var result = _service.ValidateProduct(dto);

            if (string.IsNullOrWhiteSpace(category))
            {
                Assert.Equal(
                    "Category code is required.",
                    result);
            }
            else
            {
                Assert.Equal(
                    "Category code must have format AAA999.",
                    result);
            }
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void ValidateProduct_InvalidExpiration_ReturnsError(int days)
        {
            var dto = ValidProduct();

            dto.ExpDateProd =
                DateOnly.FromDateTime(
                    DateTime.Today.AddDays(days));

            var result = _service.ValidateProduct(dto);

            Assert.Equal(
                "Expiration date must be in the future.",
                result);
        }

        [Fact]
        public void ValidateProduct_Valid_ReturnsNull()
        {
            var dto = ValidProduct();

            var result = _service.ValidateProduct(dto);

            Assert.Null(result);
        }

        private static ProductDto ValidProduct()
        {
            return new ProductDto
            {
                CodProd = "MED1234567",
                NameProd = "Ibuprofen",
                PriceProd = 10,
                ExpDateProd = DateOnly.FromDateTime(
                    DateTime.Today.AddYears(1)),
                PresentationTypeProd = "Tablets",
                SizeProd = 500,
                CodeCat = "MED001",
                StockQty = 100
            };
        }
    }
}
