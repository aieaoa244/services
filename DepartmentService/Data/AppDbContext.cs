using DepartmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    public DbSet<Department> Departments { get; set; } = null!;
}
