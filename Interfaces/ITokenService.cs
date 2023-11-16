using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using It_Supporter.Models;
using Microsoft.AspNetCore.Identity;

namespace It_Supporter.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration builder);
        string GenerateRefreshToken();
        //Task<AuthResult> refresh(RefreshToken rftoken, IConfiguration builder);
        //ClaimsPrincipal getclaimsprincipalfromexprisetime(string token, IConfiguration builder);

        //Task<List<IdentityRole>?> getallroles();
    }
}