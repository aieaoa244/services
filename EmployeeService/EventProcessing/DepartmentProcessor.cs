using System.Text.Json;
using DepartmentService.Models;
using EmployeeService.Data;
using EmployeeService.Models;
using Mapster;

namespace EmployeeService.EventProcessing;

public static class DepartmentProcessor
{
    public static async Task CreateDepartment(IServiceScopeFactory scopeFactory,
        string departmentPublishedMessage)
    {
        var departmentPublishDto = JsonSerializer.Deserialize<DepartmentPublishDto>(departmentPublishedMessage);
        if (departmentPublishDto == null)
        {
            Console.WriteLine(">> Could not deserialize message");
            throw new Exception(nameof(departmentPublishedMessage));
        }

        using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
        try
        {
            var department = departmentPublishDto.Adapt<Department>();
            if (await repository.ExternalDepartmentExists(department.ExternalId))
            {
                Console.WriteLine(">> Department already exists");
            }
            else
            {
                repository.CreateDepartment(department);
                await repository.SaveChangesAsync();
                Console.WriteLine(">> Department created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Department creating error: {ex.Message}");
        }
    }

    public static async Task UpdateDepartment(IServiceScopeFactory scopeFactory,
        string departmentPublishedMessage)
    {
        var departmentPublishDto = JsonSerializer.Deserialize<DepartmentPublishDto>(departmentPublishedMessage);
        if (departmentPublishDto == null)
        {
            Console.WriteLine(">> Could not deserialize message");
            throw new Exception(nameof(departmentPublishedMessage));
        }

        using var scope = scopeFactory.CreateAsyncScope();
        var repository = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
        try
        {
            var department = departmentPublishDto.Adapt<Department>();
            var departmentFromDb = await repository.GetExternalDepartment(department.ExternalId);
            if (departmentFromDb == null)
            {
                Console.WriteLine("Department not exists");
            }
            else
            {
                departmentFromDb.Name = department.Name;
                await repository.SaveChangesAsync();
                Console.WriteLine(">> Department updated");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($">> Department updating error: {ex.Message}");
        }
    }
}