using System;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NRedisStack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

namespace It_Supporter.Repository
{
    public class UserAccountRepo : IUserAccount, ITokenService
    {
        private readonly UserAccountContext _context;
        

        private readonly UserManager<IdentityUser> _usermanager;

        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly HttpContext _http;
        public UserAccountRepo(UserAccountContext context, IHttpContextAccessor http, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _http = http.HttpContext;

            _usermanager = userManager;
            _rolemanager = roleManager;
        }


        public async Task<List<IdentityRole>?> GetAllRoles() {
            var roles = _rolemanager.Roles.ToList();
            Console.WriteLine(roles);
            return roles;
        }



        public async Task<KeyValuePair<int,string>> createAccount(UserAccount accregister, string role)
        {
            try
            {
                //check user exits.
                var UserExits = await _usermanager.FindByEmailAsync(accregister.Email);
                //exits
                if (UserExits != null)
                {
                    KeyValuePair<int, string> rsRepo = new KeyValuePair<int, string>(
                        403, "User alread exits!"
                    );
                    return rsRepo;
                }
                //Not exits
                else
                {
                    IdentityUser user = new IdentityUser()
                    {
                        Email = accregister.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = accregister.Username
                    };
                    //create async account
                    IdentityResult result = await _usermanager.CreateAsync(user, accregister.Password);
                    if (!result.Succeeded)// not successfully
                    {
                        return new KeyValuePair<int, string>(500, "Faile when creating account");
                    }
                    else
                    {
                        //add role to user
                        await _usermanager.AddToRoleAsync(user, role);
                        return new KeyValuePair<int, string>(200, "Create account successfully!");
                    }
                }
            }
            catch
            {
                return new KeyValuePair<int, string>(500, "Faile when creating account");
            }
        }
        //    public string GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration builder, UserAccount account)
        //    {
        //        var tokenOptions = new JwtSecurityToken(
        //            issuer: builder["Jwt:Issuer"],
        //            audience: builder["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddHours(1),
        //            notBefore: DateTime.UtcNow,
        //            signingCredentials: new SigningCredentials(
        //                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder["Jwt:Key"])),
        //                SecurityAlgorithms.HmacSha256)
        //        );
        //        string tokenstring = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //        return tokenstring;
        //    }
        //    public string GenerateRefreshToken()
        //    {
        //        var randomByte = new byte[32];
        //        using (var randomb = RandomNumberGenerator.Create())
        //        {
        //            randomb.GetBytes(randomByte);
        //            return System.Convert.ToBase64String(randomByte);
        //        }
        //    }

        //    public ClaimsPrincipal GetClaimsPrincipalFromExpriseTime(string token, IConfiguration builder)
        //    {
        //        var tokenValidationParamerters = new TokenValidationParameters {
        //            ValidateAudience = false,
        //            ValidateIssuer = false,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder["Jwt:Key"])),
        //            ValidateLifetime = false,
        //        };
        //        var tokenHandle = new JwtSecurityTokenHandler();
        //        SecurityToken securityToken;
        //        var principal = tokenHandle.ValidateToken(token,tokenValidationParamerters, out securityToken);
        //        var jwtSecurityToken = securityToken as JwtSecurityToken;

        //        if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
        //            throw new SecurityTokenException("Invalid token");
        //        }
        //        return principal;
        //    }

        //    public async  Task<UserAccount> GetUserAccounts(string username, string password)
        //    {
        //        try
        //        {
        //            var user = await _context.UserAccount.Where(p => p.Username == username && p.Password == password).FirstAsync();
        //            return user;
        //        } catch(Exception ex)
        //        {
        //            return null;
        //        }
        //    }
        //    public async Task<AuthResult> Login(Login login, IConfiguration builder)
        //    {
        //        AuthResult authResult = new AuthResult();
        //        if (login.username != null && login.password != null)
        //        {
        //            var account = await GetUserAccounts(login.username, login.password);
        //            if (account == null)
        //            {
        //                authResult.statuscode = 404;
        //                authResult.errors.Add("User has not found");
        //            }
        //            else
        //            {
        //                var claims = new List<Claim>() {
        //                    new Claim(ClaimTypes.NameIdentifier, account.Username),
        //                    new Claim(ClaimTypes.Email, account.Email),
        //                    new Claim(ClaimTypes.Role, account.Role)
        //                }; 
        //                string accessToken = GenerateAccessToken(claims,builder,account);
        //                string refreshToken = GenerateRefreshToken();
        //                // set session 
        //                ISession _session = _http.Session;
        //                string key = "MaTV";
        //                string valueId = account.MaTV;
        //                string role = "Role";
        //                string roleAccount = account.Role;
        //                _session.SetString(role,roleAccount);
        //                _session.SetString(key,valueId);
        //                _session.SetString(_session.Id,Guid.NewGuid().ToString());
        //                account.RefreshToken = refreshToken;
        //                account.RefreshTokenExpireTime = DateTime.Now.AddDays(7);
        //                int rowAffted = _context.SaveChanges();
        //                if(rowAffted > 0) { 
        //                    authResult.statuscode = 200;
        //                    authResult.message = "Login successfully!";
        //                    authResult.AccessToken = accessToken;
        //                }  
        //            }
        //        } else {
        //            authResult.statuscode = 400;
        //            authResult.message = "Login unsuccessfully!";
        //        }
        //        return authResult;
        //    }

        //    public async Task<AuthResult> Refresh(RefreshToken rftoken, IConfiguration builder)
        //    {
        //        AuthResult authResult = new AuthResult();
        //        try {
        //            var principal = GetClaimsPrincipalFromExpriseTime(rftoken.AccessToken, builder);
        //            string username = principal.Identity.Name;
        //            Console.Write(username);
        //            var user = await _context.UserAccount.SingleOrDefaultAsync(p => p.Username == username);
        //            if(user is null || user.RefreshToken != rftoken.refreshToken || user.RefreshTokenExpireTime <= DateTime.Now) {
        //                return null;
        //            } else {
        //                string newaccessstoken = GenerateAccessToken(principal.Claims,builder,user);
        //                string newfreshtoken = GenerateRefreshToken();
        //                user.RefreshToken = newfreshtoken;
        //                user.RefreshTokenExpireTime = DateTime.Now.AddDays(7);
        //                int rowAffted = _context.SaveChanges();
        //                if(rowAffted > 0 ? true : false) {
        //                    authResult.RefreshToken = newfreshtoken;
        //                    authResult.AccessToken = newaccessstoken;
        //                    authResult.statuscode = 200;
        //                    authResult.message = "Refresh Token successfully!";
        //                }
        //            }
        //            return authResult;
        //        } catch {
        //            return null;
        //        }
        //    }

    }
}