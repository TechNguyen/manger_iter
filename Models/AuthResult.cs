using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class AuthResult
    {
        public string Token {set; get;}

        public string RefreshToken {set; get;}

        public bool result {set; get;}
        public List<string> errors {set; get;}
    }
}