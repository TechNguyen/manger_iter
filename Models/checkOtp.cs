using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace It_Supporter.Models
{
    public class checkOtp
    {
        public string email {set; get;}
        [Required]
        public int Otp {set; get;}
    }
}