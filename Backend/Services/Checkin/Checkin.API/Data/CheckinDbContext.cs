using Checkin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Checkin.API.Data;

public class CheckinDbContext : DbContext
{
    public CheckinDbContext(DbContextOptions<CheckinDbContext> options) : base(options) { }
    public DbSet<CheckIn> CheckIns => Set<CheckIn>();
}
