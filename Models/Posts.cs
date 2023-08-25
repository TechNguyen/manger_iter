
using System.ComponentModel.DataAnnotations;

namespace It_Supporter.Models
{
    public class Posts
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [DataType(DataType.DateTime)]
        [Required]
        public DateTime CreatePost { set; get; }
        public string Content { get; set; }
    }
}
