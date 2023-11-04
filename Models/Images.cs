using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace It_Supporter.Models
{
    public class Images
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {set; get;}
        [Required]
        [Column(TypeName = "int")]
        public int postId {set; get;}
        [Column(TypeName = "varchar(max)")]
        public string url {set; get;}
        [Column(TypeName = "ntext")]
        public string description {set; get;}
    }
}