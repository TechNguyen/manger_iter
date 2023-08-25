using It_Supporter.Models;
using Microsoft.EntityFrameworkCore;


namespace It_Supporter.DataContext
{
    public class ThanhVienContext : DbContext
    {
        public ThanhVienContext(DbContextOptions<ThanhVienContext> options) : base(options) 
        {
        }
       
        public DbSet<ThanhVien> THANHVIEN { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<Posts> Posts { get; set; }

        public DbSet<Comments> Comments { get; set; }

        public DbSet<TechnicalEvents> TechnicalEvents {set; get;}
        public DbSet<formTechUsers> formTechUsers {set; get;}
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                  
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
