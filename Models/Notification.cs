using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class Notification
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotiId {set; get;}
        [Column(TypeName = "char(10)")]
        [Required]
        public string FromUserId {set; get;}
        [Required]
        [Column(TypeName = "char(10)")]
        public string ToUserId {set; get;}
        [Column(TypeName = "varchar(455)")]
        public string? NotiHeader {set; get;}
        [Column(TypeName = "varchar(455)")]
        public string NotiBody {set; get;}
        [Column(TypeName = "tinyint")]
        public int isRead {set; get;}
        [Column(TypeName = "datetime")]
        public DateTime CreateDate {set; get;}
    }
}