using System.Xml;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.ComponentModel;

namespace It_Supporter.Models
{

    [Table("TechEvents")]
    public class TechEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int id {set; get;}
        [Required]
        [Column(TypeName = "nvarchar(300)")]
        public string address {set; get;}
        [Required]
        [Column(TypeName = "date")]
        public DateTime techday {set;get;}
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime timestart {set; get;}
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime timeend {set; get;}

        [Column(TypeName = "nvarchar(50)")]
        public string? status {set;get;} = "Chưa diễn ra";
    }
}