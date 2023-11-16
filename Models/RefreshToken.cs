using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class RefreshTokens
    {
        [Required]
        [Key]
        [Column(TypeName = "varchar(450)")]
        public string UserId { get; set; }
        [Required]
        [Column(TypeName = "varchar(40)")]
        public string RefreshToken { set;get;}
        [Required]
        public DateTime ExpriseTime { set; get; }
    }
}