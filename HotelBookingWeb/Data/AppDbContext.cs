using HotelBookingWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Amenity> Amenities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔗 Hotel → Rooms (1:M)
            modelBuilder.Entity<Hotel>()
                .HasMany(h => h.Rooms)
                .WithOne(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            // 🔗 Room → Booking (1:M)
            modelBuilder.Entity<Booking>()
                .HasOne<Room>()
                .WithMany()
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔗 User → Booking (1:M)
            modelBuilder.Entity<Booking>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 🔐 Unique Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 🔐 Booking Number unique
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.BookingNumber)
                .IsUnique();
        }
    }
}