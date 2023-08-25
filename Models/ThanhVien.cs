using It_Supporter.Convert;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace It_Supporter.Models
{
	public class ThanhVien
	{
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
		[DataType(DataType.DateTime)]
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
		public int namvaohoc { set; get; }
    }
}
