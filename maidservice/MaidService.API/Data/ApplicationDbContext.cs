using Microsoft.EntityFrameworkCore;
using MaidService.API.Models;

namespace MaidService.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<MaidProfile> MaidProfiles { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // MaidProfile configuration
            modelBuilder.Entity<MaidProfile>()
                .HasOne(mp => mp.User)
                .WithOne()
                .HasForeignKey<MaidProfile>(mp => mp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking configuration
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Client)
                .WithMany()
                .HasForeignKey(b => b.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Maid)
                .WithMany()
                .HasForeignKey(b => b.MaidId)
                .OnDelete(DeleteBehavior.Restrict);

            // Review configuration
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Booking)
                .WithOne()
                .HasForeignKey<Review>(r => r.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
