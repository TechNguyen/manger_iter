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


        private readonly ThanhVienContext _thanhviencontext;
            private readonly UserManager<IdentityUser> _usermanager;

            private readonly RoleManager<IdentityRole> _rolemanager;

            private readonly SignInManager<IdentityUser> _signinmanager;   

            private readonly HttpContext _http;
            public UserAccountRepo(UserAccountContext context, 
                IHttpContextAccessor http,
                UserManager<IdentityUser> userManager,
                RoleManager<IdentityRole> roleManager,
                SignInManager<IdentityUser> signinmamager,
                ThanhVienContext thanhVienContext
                )
            {
                _context = context;
                _http = http.HttpContext;
                _usermanager = userManager;
                _rolemanager = roleManager;
                _signinmanager = signinmamager;
                _thanhviencontext = thanhVienContext;
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
                        if(await _rolemanager.RoleExistsAsync(role))
                        {
                            IdentityResult result = await _usermanager.CreateAsync(user, accregister.Password);
                            if (!result.Succeeded)// not successfully
                            {
                                string error = "";
                                foreach (var item in result.Errors)
                                {
                                    error += item.Description;
                                }
                                return new KeyValuePair<int, string>(404, error);
                            }
                            else
                            {
                                //add role to user
                                IdentityResult rs = await _usermanager.AddToRoleAsync(user, role);
                                return rs.Succeeded ? new KeyValuePair<int, string>(200, "Create account successfully!") : new KeyValuePair<int, string>(400, "Create unsucessfully!");
                            }
                        } else
                        {
                            return new KeyValuePair<int, string>(403, $"Role {role} does not exits");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new KeyValuePair<int, string>(500, ex.Message);

                }
            }
        public string GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration builder)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: builder["Jwt:Issuer"],
                audience: builder["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(2),
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

        public ClaimsPrincipal GetClaimsPrincipalFromExpriseTime(string token, IConfiguration builder)
        {
            var tokenValidationParamerters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder["Jwt:Key"])),
                ValidateLifetime = false,
            };
            var tokenHandle = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandle.ValidateToken(token, tokenValidationParamerters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> GetUserAccounts(Login acclogin)
        {
            try
            {
                var user = await _signinmanager.PasswordSignInAsync(acclogin.username, acclogin.password, false, false);
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<AuthResult> Login(Login login, IConfiguration builder)
        {
            AuthResult authResult = new AuthResult();
            if (login.username != null && login.password != null)
            {
                var userCheck = await _usermanager.FindByNameAsync(login.username);
                if (userCheck == null)
                {
                    authResult.statuscode = 404;
                    authResult.message = "Username not exits";
                    return authResult;
                }
                var resultaccount = await GetUserAccounts(login);
                if (!resultaccount.Succeeded)
                {
                    authResult.statuscode = 404;
                    authResult.message = "Password not corect!";
                    return authResult;
                } else
                {
                    var user =  await _usermanager.FindByNameAsync(login.username);
                    var roleUser = await _usermanager.GetRolesAsync(user);
                    var claims = new List<Claim>() {
                            new Claim(ClaimTypes.NameIdentifier, user.UserName),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role,  roleUser[0])
                        };
                    string accessToken = GenerateAccessToken(claims, builder); 
                    string refreshToken = GenerateRefreshToken();
                   // set session
                    ISession _session = _http.Session;
                    string role = "Role";
                    _session.SetString(role, roleUser[0]);
                    _session.SetString(_session.Id, Guid.NewGuid().ToString());
                    authResult.statuscode = 200;
                    authResult.message = "Login successfully!";
                    authResult.AccessToken = accessToken;
                    authResult.RefreshToken = refreshToken;


                    //add RefreshToken
                    RefreshTokens refreshtoken = new RefreshTokens();

                    refreshtoken.UserId = user.Id;
                    refreshtoken.RefreshToken = refreshToken;
                    refreshtoken.ExpriseTime = DateTime.Now.AddDays(3);
                    _thanhviencontext.RefreshTokens.Add(refreshtoken);

                    int count = await _thanhviencontext.SaveChangesAsync();

                    Console.WriteLine(count);           
                    return authResult;
                }
            }    
            authResult.statuscode = 400;
            authResult.message = "Username and Password required";
            return authResult;
        }

        //public async Task<AuthResult> Refresh(RefreshToken rftoken, IConfiguration builder)
        //{
        //    AuthResult authResult = new AuthResult();
        //    try
        //    {
        //        var principal = GetClaimsPrincipalFromExpriseTime(rftoken.AccessToken, builder);
        //        string username = principal.Identity.Name;
        //        Console.Write(username);
        //        var user = await _context.UserAccount.SingleOrDefaultAsync(p => p.Username == username);
        //        if (user is null || user.RefreshToken != rftoken.refreshToken || user.RefreshTokenExpireTime <= DateTime.Now)
        //        {
        //            return null;
        //        }
        //        else
        //        {
        //            string newaccessstoken = GenerateAccessToken(principal.Claims, builder, user);
        //            string newfreshtoken = GenerateRefreshToken();
        //            user.RefreshToken = newfreshtoken;
        //            user.RefreshTokenExpireTime = DateTime.Now.AddDays(7);
        //            int rowAffted = _context.SaveChanges();
        //            if (rowAffted > 0 ? true : false)
        //            {
        //                authResult.RefreshToken = newfreshtoken;
        //                authResult.AccessToken = newaccessstoken;
        //                authResult.statuscode = 200;
        //                authResult.message = "Refresh Token successfully!";
        //            }
        //        }
        //        return authResult;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

    }
}