using BackgroundJobDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJobDemo.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<BackgroundJob> BackgroundJobs { get; set; }
}