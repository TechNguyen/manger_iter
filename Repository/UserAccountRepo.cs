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

namespace It_Supporter.Repository
{
    public class UserAccountRepo : IUserAccount, ITokenService
    {
        private readonly UserAccountContext _context;
        
        private readonly HttpContext _http;
        public UserAccountRepo(UserAccountContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http.HttpContext;
        }

        public async Task<bool> createAccount(User user)
        {
            try {
                UserAccount userAccount = new UserAccount();
                userAccount.Username = user.username;
                userAccount.Password = user.password;
                userAccount.Email = user.Email;
                userAccount.MaTV = user.MaTV;
                userAccount.createat = DateTime.Now;
                _context.UserAccount.Add(userAccount);
                return _context.SaveChanges() > 0 ? true : false;
            } catch  {
                return false;
            }
        }
        public string GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration builder, UserAccount account)
        {
            claims.Append(new Claim(ClaimTypes.NameIdentifier, account.Username));
            claims.Append(new Claim(ClaimTypes.Email, account.Email));
            claims.Append(new Claim(ClaimTypes.Role,account.Role));
            var tokenOptions = new JwtSecurityToken(
                issuer: builder["Jwt:Issuer"],
                audience: builder["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );
            string tokenstring = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenstring;
        }
        public string GenerateRefreshToken()
        {
            var randomByte = new byte[32];
            using (var randomb = RandomNumberGenerator.Create())
            {
                randomb.GetBytes(randomByte);
                return System.Convert.ToBase64String(randomByte);
            }
        }

        public ClaimsPrincipal GetClaimsPrincipalFromExpriseTime(string token)
        {
            var tokenValidationParamerters = new TokenValidationParameters {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("")),
                ValidateLifetime = false,
            };
            var tokenHandle = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandle.ValidateToken(token,tokenValidationParamerters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async  Task<UserAccount> GetUserAccounts(string username, string password)
        {
            try
            {
                var user = await _context.UserAccount.Where(p => p.Username == username && p.Password == password).FirstAsync();
                return user;
            } catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<AuthResult> Login(Login login, IConfiguration builder)
        {
            AuthResult authResult = new AuthResult();
            if (login.username != null && login.password != null)
            {
                var account = await GetUserAccounts(login.username, login.password);
                if (account == null)
                {
                    authResult.statuscode = 404;
                    authResult.errors.Add("User has not found");
                }
                else
                {
                    IEnumerable<Claim> claims = new List<Claim>(); 
                    string accessToken = GenerateAccessToken(claims,builder,account);
                    string refreshToken = GenerateRefreshToken();
                    // set session 
                    ISession _session = _http.Session;
                    string key = "MaTV";
                    string valueId = account.MaTV;
                    string role = "Role";
                    string roleAccount = account.Role;
                    _session.SetString(role,roleAccount);
                    _session.SetString(key,valueId);
                    _session.SetString(_session.Id,Guid.NewGuid().ToString());
                    account.AccessToken = accessToken;
                    account.RefreshToken = refreshToken;
                    account.RefreshTokenExpireTime = DateTime.Now.AddDays(7);
                    int rowAffted = _context.SaveChanges();
                    if(rowAffted > 0) { 
                        authResult.statuscode = 200;
                        authResult.message = "Login successfully!";
                        authResult.AccessToken = accessToken;
                    }  
                }
            } else {
                authResult.statuscode = 400;
                authResult.message = "Login unsuccessfully!";
            }
            return authResult;
        }

        public async Task<AuthResult> Refresh(RefreshToken rftoken, IConfiguration builder)
        {
            AuthResult authResult = new AuthResult();
            try {
                var principal = GetClaimsPrincipalFromExpriseTime(rftoken.AccessToken);
                string username = principal.Identity.Name;
                Console.Write(username);
                var user = await _context.UserAccount.SingleOrDefaultAsync(p => p.Username == username);
                if(user is null || user.RefreshToken != rftoken.refreshToken || user.RefreshTokenExpireTime <= DateTime.Now) {
                    return null;
                } else {
                    string newaccessstoken = GenerateAccessToken(principal.Claims,builder,user);
                    string newfreshtoken = GenerateRefreshToken();
                    user.AccessToken = newaccessstoken;
                    user.RefreshToken = newfreshtoken;
                    user.RefreshTokenExpireTime = DateTime.Now.AddDays(7);
                    int rowAffted = _context.SaveChanges();
                    if(rowAffted > 0 ? true : false) {
                        authResult.RefreshToken = newfreshtoken;
                        authResult.AccessToken = newaccessstoken;
                        authResult.statuscode = 200;
                        authResult.message = "Refresh Token successfully!";
                    }
                }
                return authResult;
            } catch {
                return null;
            }
        }
        
    }
}