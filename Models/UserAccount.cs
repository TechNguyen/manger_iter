using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace It_Supporter.Models
{
    public class UserAccount
    {
        [Required(ErrorMessage = "Username is required")]
        [Column(TypeName ="varchar(100)")]
        [Key]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [Column(TypeName = "varchar(20)")]
        [MinLength(12, ErrorMessage = "Password must more than 12 characters")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not formated email type")]
        [Column(TypeName = "varchar(255)")]
        public string Email { get; set; }

    }
}
