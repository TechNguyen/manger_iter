using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class TechnicalEvents
    {
        [Required]
        [Key]
        public int IdTech { set; get;}
        public DateTime createAt {set; get;}
        [StringLength(255)]
        public string Address {set; get;}
        [DefaultValue(7)]
        public int startTime {set; get;}
        [DefaultValue(17)]
        public int endTime {set; get;}
    }
}