using System.ComponentModel.DataAnnotations;

namespace It_Supporter.Models
{
    public class Comments
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public int CreatedComment { get; set; }
        public string Content { get; set; }
    }
}
