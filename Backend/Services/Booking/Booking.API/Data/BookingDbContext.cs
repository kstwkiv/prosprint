using Booking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.API.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<BookingRecord> Bookings => Set<BookingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<BookingRecord>().OwnsMany(b => b.PassengerDetails);
}
