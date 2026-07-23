using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Entities;

namespace TaskManagement.API.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<TaskItem> Tasks { get; set; }
}