using It_Supporter.Convert;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace It_Supporter.Models
{
	public class ThanhVien
	{
		[Required]
		[Key]
		[StringLength(10)]
		public string MaTV { set; get; }
		[Required]
		[StringLength(255)]
		public string TenTv { set; get; }
		[Required]
		[StringLength(10)]
		public string Khoahoc { set; get; }
		[Required]
		[StringLength(10)]
		public string Nganhhoc { set; get; }
		[Required]
		[StringLength(10)]
		public string SoDT { set; get; }
		[Required]
		[Column(TypeName = "date")]
		public DateTime NgaySinh {set; get;}
		[Required]
		[StringLength(50)]
		public string DiaChi { set; get; }
		[Required]
		[StringLength(20)]
		public string Chucvu { set; get; }
		[Required]
		[StringLength(255)]
		public string Email { set; get; }
		[Required]
		[Column(TypeName = "int")]
		public int namvaohoc { set; get; }
		public string? Ban {set; get;}
		[Column(TypeName = "tinyint")]
		public int? deleted {set; get;} = 0;



    }
}
