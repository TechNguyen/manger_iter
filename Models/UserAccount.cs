using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace It_Supporter.Models
{
    public class UserAccount
    {
        [Required]
        [StringLength(255)]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [StringLength(355)]
        public string Email { get; set; }
        [Key]
        [Required]
        [StringLength(10)]
        [ForeignKey("MaTV")]
        public string MaTV { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string? Role { get; set; }
        [Column(TypeName = "int")]
        public int? forgetOtp {set; get;}

        [DataType(DataType.DateTime)]
        public DateTime? createat {set; get;}

        [Column(TypeName = "varchar(300)")]
        public string? AccessToken {set; get;}
        [Column(TypeName = "varchar(300)")]
        public string? RefreshToken {set;get;}
        [Column(TypeName = "datetime")]
        public DateTime? RefreshTokenExpireTime {set;get;}
    }
}
