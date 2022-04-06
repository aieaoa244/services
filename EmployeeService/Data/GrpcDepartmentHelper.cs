using System.Text.Json;
using EmployeeService.Models;
using EmployeeService.SyncDataServices.Grpc;

namespace EmployeeService.Data;

public static class GrpcDepartmentHelper
{
    public static async Task PullDepartments(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateAsyncScope();
        var grpcClient = serviceScope.ServiceProvider.GetRequiredService<IDepartmentDataClient>()
            ?? throw new NullReferenceException(nameof(IDepartmentDataClient));
        var departments = grpcClient.GetDepartments();

        var repository = serviceScope.ServiceProvider.GetRequiredService<IEmployeeRepository>()
            ?? throw new NullReferenceException(nameof(IEmployeeRepository));
        
        await AddDepartments(repository, departments);
    }

    private static async Task AddDepartments(IEmployeeRepository repository, IEnumerable<Department> departments)
    {
        Console.WriteLine(">> Adding departments");
        Console.WriteLine($"{JsonSerializer.Serialize(departments)}");

        foreach (var department in departments)
        {
            if(!await repository.ExternalDepartmentExists(department.ExternalId))
            {
                repository.CreateDepartment(department);
            }
            await repository.SaveChangesAsync();
        }
    }
}