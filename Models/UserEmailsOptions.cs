namespace It_Supporter.Models
{
    public class UserEmailsOptions
    {
        public List<string> toEmails { set; get; }
        public string subject { set; get; }
        public string body { set; get; }
        public List<KeyValuePair<string, string>> keyValuePairs { set; get; }
    }
}
