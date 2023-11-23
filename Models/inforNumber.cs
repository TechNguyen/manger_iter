using System.ComponentModel.DataAnnotations.Schema;

namespace It_Supporter.Models
{
    public class inforNumber
    {
        [Column(TypeName = "char(10)")]
        public string Khoahoc { get; set; }
        public int countMember { get; set; }

    }
}
