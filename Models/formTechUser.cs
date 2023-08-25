using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace It_Supporter.Models
{
    public class formTechUser
    {
        [Required]
        [Key]
        public int IdTech {set; get;}
        [StringLength(255)]
        [Required]
        public string username {set; get;}
        [StringLength(10)]
        [Required]
        public string phonenumber {set;get;}
        [Required]
        public string services {set;get;}
        [Required]
        [StringLength(255)]
        public string TechnicalName {set; get;}
        [StringLength(30)]
        public string status {set; get;}
    }
}