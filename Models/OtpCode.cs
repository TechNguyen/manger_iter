using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class OtpCode
    {
        public int Otp {set; get;}
        [DataType(DataType.DateTime)]
        public DateTime TimeStamp {set; get;}
    }
}