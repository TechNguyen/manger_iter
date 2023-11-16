using System.Net.Mail;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;
using Microsoft.AspNetCore.Identity;

namespace It_Supporter.Interfaces
{
    public interface IEmaiLService
    {
        //doc noi dung email
        string GetEmailBody(string templatename);
        //sendemail
        Task<bool> SendMailToReset(string email);
        Task SendToEmail (UserEmailOption userEmailOption);
        Task<IdentityResult> ChangePassWord(ResetPassword resetmodel);


    }
}