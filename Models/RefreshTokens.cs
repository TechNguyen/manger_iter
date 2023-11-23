using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace It_Supporter.Models
{
    public class RefreshTokens
    {
        [Required]
        [Key]
        [Column(TypeName = "varchar(450)")]
        public string UserId { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string RefreshToken { set; get; }
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime ExpriseTime { set; get; }
    }
}
