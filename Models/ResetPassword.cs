using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace It_Supporter.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "This field is required")]
        public string userId { get; set; }
        public string token { get; set; }
        [Required, DataType(DataType.Password)]
        public string newPassword { get; set; }
        [Required, DataType(DataType.Password)]
        public string confirmpass { get; set; }
    }
}
