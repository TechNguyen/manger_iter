using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class PostsUpload
    {
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
        public FileUpload fileUpload {set; get;}
    }
}