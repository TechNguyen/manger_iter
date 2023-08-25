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

namespace It_Supporter.Repository
{
    public class SendEmail : IEmaiLService
    {
        private const string templatePath = @"Template/{0}.html";
        private readonly UserAccountContext _userAccountContext;
        private readonly SMTP _smtp;
        public SendEmail(UserAccountContext userAccountContext, IOptions<SMTP> options ) {
            _userAccountContext = userAccountContext;
            _smtp = options.Value; 
        }
        // doc noi dung gui email
        public string GetEmailBody(string templatename) {
            var bodyEmail = File.ReadAllText(string.Format(templatePath, templatename));
            return bodyEmail;
        }
        //dynamic data in tempalte
        public string UpdatePlaceHolder(string text, List<KeyValuePair<string, int>> keyValuePairs) {
            if(!string.IsNullOrEmpty(text) && keyValuePairs != null) {
                foreach(var placeholder in keyValuePairs) {
                    if(text.Contains(placeholder.Key)) {
                        text = text.Replace(placeholder.Key, placeholder.Value.ToString());
                    }
                }
            }
            return text;
        } 
        // tao model to send
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
        // generta otp
        public OtpCode GenerateOtp(string secretkey) {
            var totp = new Totp(Base32Encoding.ToBytes(secretkey), mode: OtpHashMode.Sha256, totpSize: 4);
            DateTime expiration = DateTime.UtcNow.AddMinutes(2);
            string otp = totp.ComputeTotp(expiration);
            OtpCode otpCode = new OtpCode {
                Otp = Int32.Parse(otp),
                TimeStamp = DateTime.UtcNow
            };
            return otpCode;
        }       
        //send email
        public async Task SendToTest(UserEmailOption userEmailOption, IConfiguration builder) {
            var otpCode = GenerateOtp(builder["Secretkey"]);
            List<KeyValuePair<string,int>> keyValuePairCode = new List<KeyValuePair<string, int>>(){
                new KeyValuePair<string, int>("{{forget_otp}}",  otpCode.Otp)
            };
            userEmailOption.keyValuePairs = keyValuePairCode;
            userEmailOption.body = UpdatePlaceHolder(GetEmailBody("TestEmail"), userEmailOption.keyValuePairs);
            updateOtp(builder,userEmailOption,otpCode);
            await SendToEmail(userEmailOption);
        }

        //add otp to db
        public async Task updateOtp(IConfiguration builder, UserEmailOption userEmailOption, OtpCode otpCode) {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open(); 
            string procedurename = "dbo.SP_GenerateOtp";
            var emailParams = new SqlParameter("@email", SqlDbType.NVarChar) {
                Direction = ParameterDirection.Input,
                Value = userEmailOption.toEmails
            };
            var forGetOtp = new SqlParameter("@forgetOtp", SqlDbType.Int) {
                Direction = ParameterDirection.Input,
                Value = otpCode.Otp
            };
            var createatOtp = new SqlParameter("@createtime", SqlDbType.DateTime) {
                Direction = ParameterDirection.Input,
                Value = otpCode.TimeStamp
            };
            using (SqlCommand command = new SqlCommand(procedurename,connection))
            {   
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(emailParams);
                command.Parameters.Add(forGetOtp);
                command.Parameters.Add(createatOtp);

                command.ExecuteNonQuery();
            }
        }


        //get MaTv,Password new
        public OtpSend CheckOtp(checkOtp check, IConfiguration builder) {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();
            string procedurename = "dbo.SP_checkOtp";
            var emailParams = new SqlParameter("@email", SqlDbType.NVarChar, 255) {
                Direction = ParameterDirection.Input,
                Value = check.email
            };
            var forGetOtp = new SqlParameter("@forgetOtp", SqlDbType.Int) {
                Direction = ParameterDirection.Input,
                Value = check.Otp
            };
            var MatvParams = new SqlParameter("@MaTV", SqlDbType.Char, 10) {
                Direction = ParameterDirection.Output,
            };
            var StatusCode = new SqlParameter("@statusCode", SqlDbType.Int) {
                Direction = ParameterDirection.Output,
            };
            var createatOtp = new SqlParameter("@createat", SqlDbType.DateTime) {
                Direction = ParameterDirection.Output,
            };
            OtpSend resetotp = new OtpSend();
            using (SqlCommand command = new SqlCommand(procedurename,connection)) {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(emailParams);
                command.Parameters.Add(forGetOtp);
                command.Parameters.Add(MatvParams);
                command.Parameters.Add(StatusCode);
                command.Parameters.Add(createatOtp);


                command.ExecuteNonQuery();
                // check timeline
                string MaTv = command.Parameters["@MaTV"].Value.ToString();
                int status = (int)command.Parameters["@statusCode"].Value;
                resetotp.email =check.email;
                resetotp.MaTV = MaTv;
                resetotp.Otp = check.Otp;
                if(status == 200) {
                    DateTime time = (DateTime)command.Parameters["@createat"].Value;
                    if(DateTime.Compare(time.AddMinutes(2), DateTime.UtcNow) > 0) {
                        resetotp.statusCode = status;
                    } else {
                        resetotp.statusCode = 405;
                    }
                }
                resetotp.statusCode = status;
            }
            return resetotp;


        }
        //check Ma Otp
        public bool ResetPassword(OtpSend otpSend, IConfiguration builder) {
            SqlConnection connection = new SqlConnection();

            connection.ConnectionString = builder.GetConnectionString("DefaultConnection");
            connection.Open();

            string procedurename = "dbo.SP_ResetPassword";

            var emailParams = new SqlParameter("@email", SqlDbType.NVarChar, 355) {
                Direction = ParameterDirection.Input,
                Value = otpSend.email
            };
            var MatvParams = new SqlParameter("@MaTV", SqlDbType.Char, 10) {
                Direction = ParameterDirection.Input,
                Value = otpSend.MaTV
            };
            var PasswordParams = new SqlParameter("@password", SqlDbType.NVarChar) {
                Direction = ParameterDirection.Input,
                Value = otpSend.Password
            };
            var returncode = new SqlParameter("@returncode", SqlDbType.Int) {
                Direction = ParameterDirection.Output,
            };
            using (SqlCommand command = new SqlCommand(procedurename,connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(emailParams);
                command.Parameters.Add(MatvParams);
                command.Parameters.Add(PasswordParams);
                command.Parameters.Add(returncode);

                command.ExecuteNonQuery();

                int status = (int)command.Parameters["@returncode"].Value;
                if(status == 200)  {
                     return true;
                } 
                return false;
            }
        }
    }
}