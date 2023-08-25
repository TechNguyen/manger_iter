using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface IUserAccount
    {
        UserAccount GetUserAccounts(string username,string password);
        string Login(Login loginform, IConfiguration builder);
    }
}
