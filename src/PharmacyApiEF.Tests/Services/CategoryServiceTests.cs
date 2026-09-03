using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using PharmacyApiEF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PharmacyApiEF.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _service = new CategoryService(null!);
        }

        [Theory]
        [InlineData("CAT1")]
        [InlineData("cat001")]
        [InlineData("123456")]
        [InlineData("AB1234")]
        public void ValidateCategory_InvalidCode_ReturnsError(string code)
        {
            var dto = ValidCategory();

            dto.CodeCat = code;

            var result = _service.ValidateCategory(dto);

            Assert.Equal(
                "Category code must have format AAA999.",
                result);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("AB")]
        public void ValidateCategory_InvalidName_ReturnsError(string name)
        {
            var dto = ValidCategory();

            dto.NameCat = name;

            var result = _service.ValidateCategory(dto);

            Assert.Equal(
                "Category name must contain at least 3 characters.",
                result);
        }

        [Fact]
        public void ValidateCategory_NullCategory_ReturnsError()
        {
            var result = _service.ValidateCategory(null);

            Assert.Equal(
                "Category is required.",
                result);
        }

        [Fact]
        public void ValidateCategory_MissingCode_ReturnsError()
        {
            var dto = ValidCategory();

            dto.CodeCat = null!;

            var result = _service.ValidateCategory(dto);

            Assert.Equal(
                "Category code is required.",
                result);
        }

        [Fact]
        public void ValidateCategory_MissingName_ReturnsError()
        {
            var dto = ValidCategory();

            dto.NameCat = null!;

            var result = _service.ValidateCategory(dto);

            Assert.Equal(
                "Category name is required.",
                result);
        }

        [Fact]
        public void ValidateCategory_Valid_ReturnsNull()
        {
            var dto = ValidCategory();

            var result = _service.ValidateCategory(dto);

            Assert.Null(result);
        }

        private static CategoryDto ValidCategory()
        {
            return new CategoryDto
            {
                CodeCat = "MED001",
                NameCat = "Analgesics"
            };
        }
    }
}
