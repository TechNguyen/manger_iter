using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class Notification
    {
        [Required]
        [Key]
        public int NotiId {set; get;}
        [StringLength(10)]
        public string FromUserId {set; get;}
        public string ToUserId {set; get;}
        public string NotiHeader {set; get;}
        public string NotiBody {set; get;}
        public int isRead {set; get;}
        public string Url {set; get;}
        [DataType(DataType.DateTime)]
        public DateTime CreateDate {set; get;}
    }
}