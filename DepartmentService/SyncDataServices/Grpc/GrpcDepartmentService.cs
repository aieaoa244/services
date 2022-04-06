using System.Text.Json;
using DepartmentService.Data;
using Grpc.Core;
using Mapster;

namespace DepartmentService.SyncDataServices.Grpc;

public class GrpcDepartmentService : GrpcDepartment.GrpcDepartmentBase
{
    private readonly IDepartmentsRepository _repository;

    public GrpcDepartmentService(IDepartmentsRepository repository)
    {
        _repository = repository;
    }

    public override async Task<DepartmentResponse> GetDepartments(GetDepartmentsRequest request, ServerCallContext context)
    {
        var response = new DepartmentResponse();
        var departments = await _repository.GetDepartments();

        Console.WriteLine(">> Sending departments");
        foreach (var department in departments)
        {
            Console.WriteLine($">> {JsonSerializer.Serialize(department)}");
            response.Department.Add(department.Adapt<GrpcDepartmentModel>());
        }

        return response;
    }
}