using ApplicationsChallenge.API.Models;
using System.Security.Claims;

namespace ApplicationsChallenge.API.Services
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateCredentialsAsync(string username, string password);
        Task<string> GenerateJwtTokenAsync(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
