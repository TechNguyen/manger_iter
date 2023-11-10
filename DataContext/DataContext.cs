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
        public DbSet<Posts> Posts { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Notification> Notification {set;get;}
        public DbSet<Images> Images {set;get;}
        public DbSet<Machines> Machines {get; set;}
        public DbSet<TechEvents> TechEvents {set; get;} 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Posts>()
                .HasOne<ThanhVien>()
                .WithMany()
                .HasForeignKey(p => p.authorId)
                .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity<Posts>()
                    .Ignore(e => e.id)
                    .Property(p => p.id)
                    .UseIdentityColumn()
                    .ValueGeneratedOnAdd();
                modelBuilder.Entity<Posts>()
                    .Property(p => p.createat)
                    .HasDefaultValueSql("Current_Timestamp");
                   
                modelBuilder.Entity<Comments>()
                    .Property(p => p.createat)
                    .HasDefaultValueSql("Current_Timestamp");

                modelBuilder.Entity<Machines>()
                    .Property(e => e.deleted)
                    .HasDefaultValueSql("0");
                
                modelBuilder.Entity<Machines>()
                    .Property(p => p.finished)
                    .HasDefaultValueSql("0");
                modelBuilder.Entity<Machines>()
                    .Property(e => e.serviceCharger)
                    .HasDefaultValueSql("0.00");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
