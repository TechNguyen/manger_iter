using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace It_Supporter.Models
{
    public partial class OLDMEMBER
    {
        [Key]
        [Required]
        [StringLength(10)]
        [ForeignKey("MaTv")]
        public string MaTv { set; get; }

        [Required]
        [StringLength(255)]
        public string TenTv { set; get; }

        [Required]
        [StringLength(10)]
        public string Khoa { set; get; }
    }
}
