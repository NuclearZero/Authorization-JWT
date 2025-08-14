using System.Security.Claims;

namespace AuthorizationAPI.Services
{
    public interface IJwtTokenGenerator
    {
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }

    public interface IPasswordHasher
    {
        string Hash(string password);
        bool VerifyPassword(string hashPassword, string verifiedPassword);
    }
}
