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

namespace It_Supporter.Repository
{
    public class UserAccountRepo : IUserAccount
    {
        private readonly UserAccountContext _context;
        public UserAccountRepo(UserAccountContext context)
        {
            _context = context;
        }
        public UserAccount GetUserAccounts(string username, string password)
        {
            return _context.UserAccount.Where(p => p.Username == username && p.Password == password).FirstOrDefault();
        }
        public string Login(Login login, IConfiguration builder)
        {
            var tokenstring = string.Empty;
            if (login.username != null && login.password != null)
            {
                var account = GetUserAccounts(login.username, login.password);
                if (account == null)
                {
                    tokenstring = "User not found";
                    return tokenstring;

                }
                else
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, account.Username),
                        new Claim(ClaimTypes.Email, account.Email),
                        new Claim(ClaimTypes.Role, account.Role),
                    };
                    var token = new JwtSecurityToken(
                        issuer: builder["Jwt:Issuer"],
                        audience: builder["Jwt:Audience"],
                        claims: claims,
                        expires: DateTime.UtcNow.AddDays(3),
                        notBefore: DateTime.UtcNow,
                        signingCredentials: new SigningCredentials(
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder["Jwt:Key"])),
                            SecurityAlgorithms.HmacSha256)
                    );

                    tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
                    return tokenstring;
                }
            }
            return tokenstring;
        }
    }
}
