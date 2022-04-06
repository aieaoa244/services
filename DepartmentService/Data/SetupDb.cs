using DepartmentService.Models;
using Microsoft.EntityFrameworkCore;

namespace DepartmentService.Data;

public static class SetupDb
{
    public static async Task AddTestData(IApplicationBuilder app, bool isProduction)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<AppDbContext>()
            ?? throw new NullReferenceException(nameof(AppDbContext));
        
        if (isProduction)
        {
            ApplyMigrations(context);
        }

        await AddDepartments(context);
    }

    static void ApplyMigrations(AppDbContext context)
    {
        Console.WriteLine(">> Applying migrations");
        try
        {
            context.Database.Migrate();
        }
        catch (Exception e)
        {
            Console.WriteLine($">> Migrating failed: {e.Message}");
        }
    }

    static async Task AddDepartments(AppDbContext context)
    {
        if (!context.Departments.Any())
        {
            Console.WriteLine(">> Inserting test data");
            context.AddRange(
                new Department { Name = "HR" },
                new Department { Name = "Tech" },
                new Department { Name = "Finance" }
            );
            await context.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine(">> Data already exists");
        }
    }
}
