
using DocumentFormat.OpenXml.Spreadsheet;
using Hangfire;
using It_Supporter.DataContext;
using It_Supporter.Interfaces;
using It_Supporter.Models;

namespace It_Supporter.BackGroundJob
{
    public class BirthdayJob : IbirthDay
    {
        private readonly ThanhVienContext _context;

        private readonly IEmaiLService _emaiLService;

        public BirthdayJob(
            ThanhVienContext context,
            IEmaiLService emaiLService
            )
        {
            _context = context;
            _emaiLService = emaiLService;
        }
        public async Task<bool?> sendMailHappyBirday()
        {
            try
            {
                var server = new BackgroundJobServer();

                var members = _context.THANHVIEN.Where(m => m.NgaySinh.Day == DateTime.Now.Day && m.NgaySinh.Month == DateTime.Now.Month).ToList();
                if( members.Count > 0 )
                {
                    //get list email
                    List<string> emails = new List<string>();
                    members.ForEach(e => emails.Add(e.Email));
                    emails.ForEach(async e =>
                    {
                        UserEmailOption userEmailsOptions = new UserEmailOption();

                        userEmailsOptions.subject = "Happy Birthday to you";
                        userEmailsOptions.toEmails = e;
                        userEmailsOptions.body = _emaiLService.UpdatePlaceHolder(_emaiLService.GetEmailBody("mailHappyBirthDay"), new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("{{name}}", _context.THANHVIEN.Where(p => p.Email == e).FirstOrDefault().TenTv)
                        });

                        await _emaiLService.SendToEmail(userEmailsOptions);
                    });
                }
                return true;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
