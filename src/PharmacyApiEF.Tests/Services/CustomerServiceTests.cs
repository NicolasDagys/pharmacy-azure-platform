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
    public class CustomerServiceTests
    {
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _service = new CustomerService(null!);
        }

        [Fact]
        public void ValidateCustomer_ValidCustomer_ReturnsNull()
        {
            // Arrange
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "John Doe",
                MailCus = "john@test.com",
                PhoneCus = "099123456"
            };

            // Act
            var result = _service.ValidateCustomer(dto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ValidateCustomer_NullCustomer_ReturnsError()
        {
            // Act
            var result = _service.ValidateCustomer(null);

            // Assert
            Assert.Equal("Customer is required.", result);
        }

        [Fact]
        public void ValidateCustomer_EmptyId_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "",
                NameCus = "John Doe",
                MailCus = "john@test.com"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal("Customer ID is required.", result);
        }

        [Fact]
        public void ValidateCustomer_InvalidIdFormat_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "ABC123",
                NameCus = "John Doe",
                MailCus = "john@test.com"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal("Invalid customer ID format.", result);
        }

        [Fact]
        public void ValidateCustomer_EmptyName_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "",
                MailCus = "john@test.com"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal("Customer name is required.", result);
        }

        [Fact]
        public void ValidateCustomer_NameTooShort_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "Jo",
                MailCus = "john@test.com"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal(
                "Customer name must contain at least 3 characters.",
                result);
        }

        [Fact]
        public void ValidateCustomer_EmptyEmail_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "John Doe",
                MailCus = ""
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal("Email is required.", result);
        }

        [Fact]
        public void ValidateCustomer_InvalidEmail_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "John Doe",
                MailCus = "invalid-email"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal("Invalid email format.", result);
        }

        [Fact]
        public void ValidateCustomer_InvalidPhone_ReturnsError()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "John Doe",
                MailCus = "john@test.com",
                PhoneCus = "099-123456"
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Equal(
                "Phone must contain only digits.",
                result);
        }

        [Fact]
        public void ValidateCustomer_EmptyPhone_ReturnsNull()
        {
            CustomerDto dto = new CustomerDto
            {
                IdCus = "123456-7",
                NameCus = "John Doe",
                MailCus = "john@test.com",
                PhoneCus = null
            };

            var result = _service.ValidateCustomer(dto);

            Assert.Null(result);
        }
    }
}
