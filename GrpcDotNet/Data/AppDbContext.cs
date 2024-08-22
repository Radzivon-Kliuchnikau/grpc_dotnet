using GrpcDotNet.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcDotNet.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<DutyTask> Tasks => Set<DutyTask>();
}