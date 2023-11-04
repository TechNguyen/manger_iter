using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2.Requests;
using It_Supporter.DataRes;

namespace It_Supporter.Models
{
    public class AuthResult : ProducerResponse
    {
        public string AccessToken {set; get;}
        public string RefreshToken {set; get;}
        public List<string> errors {set; get;}
    }
}