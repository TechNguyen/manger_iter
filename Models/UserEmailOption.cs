using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using It_Supporter.Interfaces;

namespace It_Supporter.Models
{
    public class UserEmailOption
    {
        public string toEmails {set; get;}
        public string subject {set; get;}
        public string body {set; get;}

        public List<KeyValuePair<string,int>> keyValuePairs {set; get;}
    }
}