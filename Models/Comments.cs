using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace It_Supporter.Models
{
    public class Comments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int? id {set; get;}
        [Required]
        [Column(TypeName = "char(10)")]
        public string authorId {set; get;}
        [Required]
        [Column(TypeName = "int")]
        public int postId {set; get;}
        [Column(TypeName = "datetime")]
        public DateTime? createat {set; get;} = DateTime.Now;
        [Column(TypeName = "datetime")]
        public DateTime? deleteat {set; get;} = null;
        [Required]
        [Column(TypeName = "ntext")]
        public string content {set; get;}
        [Column(TypeName = "int")]
        public int? deleted {set; get;} = 0;
        [Column(TypeName = "int")]
        public int? parentId {set; get;}
        
        [Column(TypeName = "datetime")]
        public DateTime? updateat { set; get;} = null;
    }
}