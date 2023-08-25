using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class OtpSend
    {   
        [StringLength(255)]
        [Required]
        public string email {set; get;}
        [Required]
        public int Otp {set; get;} 
        [StringLength(10)]
        public string MaTV {set; get;}
        public string Password {set; get;}
        public int statusCode {set; get;}
    }
}