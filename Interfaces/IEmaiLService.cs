using System.Net.Mail;
using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Models;

namespace It_Supporter.Interfaces
{
    public interface IEmaiLService
    {
        //doc noi dung email
        string GetEmailBody(string templatename);
        //sendemail
        Task SendToTest(UserEmailOption userEmailOption, IConfiguration builder);
        Task SendToEmail (UserEmailOption userEmailOption);
        //check nma otp
        OtpSend CheckOtp(checkOtp check, IConfiguration builder);
        bool ResetPassword(OtpSend otpSend, IConfiguration builder);
        //generate otp
        OtpCode GenerateOtp(string secretkey);
        Task updateOtp(IConfiguration builder, UserEmailOption userEmailOption, OtpCode otpCode);
    }
}