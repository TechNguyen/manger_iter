using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class User
    {
        [Required]
        [RegexStringValidator("^(?=.*[A-Z])(?=.*'\'d)(?=.*'\'W).{6,}$")]
        public string username {set; get;}
        [Required]
        [RegexStringValidator("^(?=.*[A-Z])(?=.*'\'d)(?=.*'\'W).{8,}$")]
        public string password {set;get;}
        [Required]
        [RegexStringValidator("/^'[\'w-'\'.]+@('[\'w-]+'\'.)+['\'w-]{2,4}$/g")]
        public string Email {set; get;}
        [StringLength(10)]
        [Required]
        public string MaTV {set;get;}
    }
}