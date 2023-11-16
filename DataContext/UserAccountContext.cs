using It_Supporter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace It_Supporter.DataContext
{
    public class UserAccountContext : IdentityDbContext<IdentityUser>
    {
        public UserAccountContext(DbContextOptions<UserAccountContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
            SeedRoles(modelBuilder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                    new IdentityRole() { Name = "Admin", NormalizedName = "Admin", ConcurrencyStamp = "1" },
                    new IdentityRole() { Name = "User", NormalizedName = "User", ConcurrencyStamp = "2" }
                    );
        }

    }
}

