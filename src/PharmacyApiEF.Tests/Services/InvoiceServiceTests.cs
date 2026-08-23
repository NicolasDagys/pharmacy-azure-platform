using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services;
using PharmacyApiEF.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PharmacyApiEF.Tests.Services
{
    public class InvoiceServiceTests
    {
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _service = new InvoiceService(null!);
        }

        [Fact]
        public void ValidateInvoice_NullInvoice_ReturnsError()
        {
            var result = _service.ValidateInvoice(null!);

            Assert.Equal(
                "Invoice is required.",
                result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ValidateInvoice_InvalidCustomer_ReturnsError(string id)
        {
            var dto = ValidInvoice();

            dto.IdCus = id;

            var result = _service.ValidateInvoice(dto);

            Assert.Equal(
                "Customer is required.",
                result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc")]
        public void ValidateInvoice_InvalidAddress_ReturnsError(string address)
        {
            var dto = ValidInvoice();

            dto.ShipmentAddressInv = address;

            var result = _service.ValidateInvoice(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateInvoice_NoLines_ReturnsError()
        {
            var dto = ValidInvoice();

            dto.InvoiceLines.Clear();

            var result = _service.ValidateInvoice(dto);

            Assert.Equal(
                "Invoice must contain at least one line.",
                result);
        }

        [Fact]
        public void ValidateInvoice_DuplicateProducts_ReturnsError()
        {
            var dto = ValidInvoice();

            dto.InvoiceLines.Add(
                new InvoiceLineDto
                {
                    CodProd = "MED1234567",
                    Quantity = 2
                });

            var result = _service.ValidateInvoice(dto);

            Assert.Equal(
                "Duplicate products are not allowed.",
                result);
        }

        [Fact]
        public void ValidateInvoice_Valid_ReturnsNull()
        {
            var dto = ValidInvoice();

            var result = _service.ValidateInvoice(dto);

            Assert.Null(result);
        }

        private static InvoiceCreateDto ValidInvoice()
        {
            return new InvoiceCreateDto
            {
                IdCus = "1234567-8",
                ShipmentAddressInv = "Main Street 123",
                InvoiceLines =
                {
                    new InvoiceLineDto
                    {
                        CodProd="MED1234567",
                        Quantity=2
                    }
                }
            };
        }
    }
}
