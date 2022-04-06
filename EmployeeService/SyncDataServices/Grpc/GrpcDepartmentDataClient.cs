using DepartmentService;
using EmployeeService.Models;
using Grpc.Net.Client;
using Mapster;

namespace EmployeeService.SyncDataServices.Grpc;

public class GrpcDepartmentDataClient : IDepartmentDataClient
{
    private readonly IConfiguration _configuration;

    public GrpcDepartmentDataClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<Department> GetDepartments()
    {
        Console.WriteLine($">> Calling grpc service: {_configuration["GrpcDepartmentEndpoint"]}");
        var channel = GrpcChannel.ForAddress(_configuration["GrpcDepartmentEndpoint"]);
        var client = new GrpcDepartment.GrpcDepartmentClient(channel);
        var reply = new List<Department>();

        try
        {
            var response = client.GetDepartments(new GetDepartmentsRequest());
            var departments = response.Departments.Adapt<IEnumerable<Department>>();
            if (departments != null)
            {
                reply.AddRange(departments);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($">> Grpc client error: {e.Message}");
        }

        return reply;
    }
}