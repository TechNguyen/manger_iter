
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Packaging.Signing;

namespace It_Supporter.Models
{
    public class Posts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int? id { get; set; }
        [Required]
        [Column( TypeName = "char(10)")]
        public string authorId {get;set;}
        [Column(TypeName = "ntext")]
        public string? content {get;set;}
        [Column(TypeName = "datetime")]
        public DateTime? createat {set; get;} = DateTime.Now;
        [Column(TypeName = "datetime")]
        public DateTime? updateat {set; get;}
        [Column(TypeName = "datetime")]
        public DateTime? deleteat {set; get;}
        [Column(TypeName = "tinyint")]
        public int? deleted {set;get;} = 0;
        [Column(TypeName = "varchar(300)")]
        public string? urlImage {set; get;}
    }
}
