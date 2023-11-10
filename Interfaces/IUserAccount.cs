using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface IUserAccount
    {
        //Task<UserAccount> GetUserAccounts(string username, string password);
        //Task<AuthResult> Login(Login login, IConfiguration builder);
        Task<KeyValuePair<int, string>> createAccount(UserAccount accregister, string role);
    }
}
