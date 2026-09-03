using PharmacyApiEF.DTOs;
using PharmacyApiEF.Services;

namespace PharmacyApiEF.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _service = new EmployeeService(null!);
        }

        //----------------------------------------------------
        // CreateEmployeeDto
        //----------------------------------------------------

        [Theory]
        [InlineData("")]
        [InlineData("AB")]
        public void ValidateEmployee_InvalidUsername_ReturnsError(string username)
        {
            var dto = ValidCreateEmployee();

            dto.UserEmp = username;

            var result = _service.ValidateEmployee(dto);

            if (string.IsNullOrWhiteSpace(username))
            {
                Assert.Equal(
                    "Username is required.",
                    result);
            }
            else
            {
                Assert.Equal(
                    "Username must contain at least 3 characters.",
                    result);
            }
        }

        [Fact]
        public void ValidateEmployee_UsernameTooLong_ReturnsError()
        {
            var dto = ValidCreateEmployee();

            dto.UserEmp = new string('A', 51);

            var result = _service.ValidateEmployee(dto);

            Assert.Equal(
                "Username cannot exceed 50 characters.",
                result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("A")]
        [InlineData("AB")]
        public void ValidateEmployee_InvalidName_ReturnsError(string name)
        {
            var dto = ValidCreateEmployee();

            dto.NameEmp = name;

            var result = _service.ValidateEmployee(dto);

            if (string.IsNullOrWhiteSpace(name))
            {
                Assert.Equal(
                    "Employee name is required.",
                    result);
            }
            else
            {
                Assert.Equal(
                    "Employee name must contain at least 3 characters.",
                    result);
            }
        }

        [Fact]
        public void ValidateEmployee_NameTooLong_ReturnsError()
        {
            var dto = ValidCreateEmployee();

            dto.NameEmp = new string('A', 51);

            var result = _service.ValidateEmployee(dto);

            Assert.Equal(
                "Employee name cannot exceed 50 characters.",
                result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Manager")]
        [InlineData("Developer")]
        public void ValidateEmployee_InvalidRole_ReturnsError(string role)
        {
            var dto = ValidCreateEmployee();

            dto.RoleEmp = role;

            var result = _service.ValidateEmployee(dto);

            if (string.IsNullOrWhiteSpace(role))
            {
                Assert.Equal(
                    "Role is required.",
                    result);
            }
            else
            {
                Assert.Equal(
                    "Invalid role.",
                    result);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("1234567")]
        [InlineData("password1")]
        [InlineData("PASSWORD1")]
        [InlineData("Password")]
        public void ValidateEmployee_InvalidPassword_ReturnsError(string password)
        {
            var dto = ValidCreateEmployee();

            dto.Password = password;

            var result = _service.ValidateEmployee(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateEmployee_ValidCreate_ReturnsNull()
        {
            var dto = ValidCreateEmployee();

            var result = _service.ValidateEmployee(dto);

            Assert.Null(result);
        }

        //----------------------------------------------------
        // UpdateEmployeeDto
        //----------------------------------------------------

        [Fact]
        public void ValidateUpdateEmployee_Valid_ReturnsNull()
        {
            var dto = new EmployeeUpdateDto
            {
                NameEmp = "John Smith",
            };

            var result = _service.ValidateEmployee(dto);

            Assert.Null(result);
        }

        [Fact]
        public void ValidateUpdateEmployee_NameRequired_ReturnsError()
        {
            var dto = new EmployeeUpdateDto
            {
                NameEmp = "",
            };

            var result = _service.ValidateEmployee(dto);

            Assert.Equal(
                "Employee name is required.",
                result);
        }

        //----------------------------------------------------
        // ChangePassword
        //----------------------------------------------------

        [Fact]
        public void ValidateChangePassword_CurrentPasswordRequired()
        {
            var dto = new EmployeeChangePasswordDto
            {
                CurrentPassword = "",
                NewPassword = "Password123"
            };

            var result = _service.ValidateChangePassword(dto);

            Assert.Equal(
                "Current password is required.",
                result);
        }

        [Fact]
        public void ValidateChangePassword_SamePassword_ReturnsError()
        {
            var dto = new EmployeeChangePasswordDto
            {
                CurrentPassword = "Password123",
                NewPassword = "Password123"
            };

            var result = _service.ValidateChangePassword(dto);

            Assert.Equal(
                "The new password must be different from the current password.",
                result);
        }

        [Fact]
        public void ValidateChangePassword_WeakPassword_ReturnsError()
        {
            var dto = new EmployeeChangePasswordDto
            {
                CurrentPassword = "Password123",
                NewPassword = "abc"
            };

            var result = _service.ValidateChangePassword(dto);

            Assert.NotNull(result);
        }

        [Fact]
        public void ValidateChangePassword_Valid_ReturnsNull()
        {
            var dto = new EmployeeChangePasswordDto
            {
                CurrentPassword = "Password123",
                NewPassword = "NewPassword123"
            };

            var result = _service.ValidateChangePassword(dto);

            Assert.Null(result);
        }

        //----------------------------------------------------
        // Helpers
        //----------------------------------------------------

        private static EmployeeCreateDto ValidCreateEmployee()
        {
            return new EmployeeCreateDto
            {
                UserEmp = "jsmith",
                NameEmp = "John Smith",
                Password = "Password123",
                RoleEmp = "Sales"
            };
        }
    }
}