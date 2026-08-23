namespace PharmacyApiEF.Utilities
{
    public static class PasswordPolicy
    {
        public static string? Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Password is required.";

            if (password.Length < 8)
                return "Password must contain at least 8 characters.";

            if (password.Length > 100)
                return "Password is too long.";

            if (!password.Any(char.IsUpper))
                return "Password must contain at least one uppercase letter.";

            if (!password.Any(char.IsLower))
                return "Password must contain at least one lowercase letter.";

            if (!password.Any(char.IsDigit))
                return "Password must contain at least one number.";

            return null;
        }
    }
}
