using System.Data;
using Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using OtpNet;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;
using Microsoft.Extensions.Options;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using RabbitMQ.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Resources;
using Irony.Parsing;
using ExcelDataReader.Log.Logger;

namespace It_Supporter.Repository
{
    public class SendEmail : IEmaiLService
    {
        private const string templatePath = @"Template/{0}.html";
        private readonly UserAccountContext _userAccountContext;
        private readonly SMTP _smtp;
        private readonly UserManager<IdentityUser> _userManager;


        public SendEmail(UserAccountContext userAccountContext, IOptions<SMTP> options , UserManager<IdentityUser> userManager) {
            _userAccountContext = userAccountContext;
            _smtp = options.Value;
            _userManager = userManager;
        }
        // doc noi dung gui email
        public string GetEmailBody(string templatename) {
            var bodyEmail = File.ReadAllText(string.Format(templatePath, templatename));
            return bodyEmail;
        }
        //dynamic data in tempalte
        public string UpdatePlaceHolder(string text, List<KeyValuePair<string, string>> keyValuePairs) {
            if(!string.IsNullOrEmpty(text) && keyValuePairs != null) {
                foreach(var placeholder in keyValuePairs) {
                    if(text.Contains(placeholder.Key)) {
                        text = text.Replace(placeholder.Key, placeholder.Value.ToString());
                    }
                }
            }
            return text;
        } 
        // tao model to send 1 mail
        public async Task SendToEmail(UserEmailOption userEmailOption) {
            // tao 1 mail message;
            MailMessage mail = new MailMessage {
                Subject = userEmailOption.subject,
                Body = userEmailOption.body,
                From = new MailAddress(_smtp.SenderAddress,_smtp.SenderDisplayName),
                IsBodyHtml = _smtp.IsBodyHTML
            };
            // gui den nhieu email cung 1 luc
            // foreach (var toEmail in userEmailOption.toEmails) 
            // {
            //     mail.To.Add(toEmail);
            // }

            //tao networkcreden
            mail.To.Add(userEmailOption.toEmails);

            NetworkCredential networkCredential = new NetworkCredential(_smtp.UserName,_smtp.Password);

            //tao smtopclient
            SmtpClient smtpClient = new SmtpClient() {
                Host = _smtp.Host,
                Port = _smtp.Port,
                EnableSsl = _smtp.EnableSSL,
                UseDefaultCredentials = _smtp.UseDefaultCredentials,
                Credentials = networkCredential
            };
            await smtpClient.SendMailAsync(mail);
        }

        public async Task<bool> GereratePasswordEmailToken(IdentityUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (token != null)
            {
                UserEmailOption sendEmail = new UserEmailOption();


                sendEmail.toEmails = user.Email;
                var text = GetEmailBody("SendEmail");
                sendEmail.subject = "This is email to verify email to reset password";
                sendEmail.keyValuePairs = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("{{user_name}}", user.UserName),
                    new KeyValuePair<string, string>("{{link}}", token)

                };
                sendEmail.body = UpdatePlaceHolder(text,sendEmail.keyValuePairs);
                 await SendToEmail(sendEmail);
                return true;
            }
            return false;
        }

        public async Task<bool> SendMailToReset(string email)
        {
            try
            {
                IdentityUser user = await _userManager.FindByEmailAsync(email);  
                if (user == null)
                {
                    return false;
                }
                else
                {
                    var res = await GereratePasswordEmailToken(user);
                    return res;
                }
            } catch
            {
                return false;
            }
        }
        public async Task<IdentityResult> ChangePassWord(ResetPassword resetmodel)
        {
            try
            {
                IdentityUser user = await _userManager.FindByIdAsync(resetmodel.userId);
                if (user != null)
                {
                    var rs = await _userManager.ResetPasswordAsync(user, resetmodel.token, resetmodel.newPassword);
                    return rs;
                }
                return null;
            } catch (Exception ex )
            {
                return null;
            }
        }
    }
}