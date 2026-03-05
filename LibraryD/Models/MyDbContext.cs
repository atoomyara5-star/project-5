using Microsoft.EntityFrameworkCore;

namespace LibraryD.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Borrowing> Borrowings { get; set; }
        public DbSet<Blacklist> Blacklists { get; set; }   // ✅ مهم
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomReservation> RoomReservations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
    }
} 