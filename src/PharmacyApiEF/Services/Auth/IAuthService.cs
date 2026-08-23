using PharmacyApiEF.DTOs;
using PharmacyApiEF.Models;
using System.Security.Claims;

namespace PharmacyApiEF.Services.Auth
{
    public interface IAuthService
    {
        Task<string?> Login(LoginDto dto); //el string que devuelve es el token, si es null es porque no se pudo loguear
        Task<Employee?> GetCurrentUser(ClaimsPrincipal user);
    }
}
