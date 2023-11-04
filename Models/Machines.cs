using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class Machines
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int id {set; get;}
        [Required]
        [Column(TypeName = "nvarchar(50)")] 
        public string customername {set; get;}
        [Required]
        [Column(TypeName = "char(10)")]
        public string phonenumber {set; get;}
        [Column(TypeName = "nvarchar(255)")]
        public string? machine_status {set;get;}
        [Required]
        public int techId {set; get;}
        [Required]
        [Column(TypeName = "nvarchar(255)")]
        public string services {set; get;}
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime machinesgetat {set;get;}
        [Column(TypeName = "char(10)")]
        public string? TesterId {set; get;}
        [Column(TypeName = "char(10)")]
        public string? Technical {set;get;}

        [Column(TypeName = "tinyint")]
        public int deleted {set; get;}
        [Column(TypeName = "money")]
        [Required]
        public decimal serviceCharger {set;get;}
        [Column(TypeName = "ntext")]
        public string? note {set;get;}

        [Column(TypeName = "tinyint")]
        [Required]
        public int finished {set;get;}
    }
}