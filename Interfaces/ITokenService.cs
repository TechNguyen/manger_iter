using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration builder, UserAccount account);
        string GenerateRefreshToken();
       Task<AuthResult> Refresh(RefreshToken rftoken, IConfiguration builder);
        ClaimsPrincipal GetClaimsPrincipalFromExpriseTime(string token);
    }
}