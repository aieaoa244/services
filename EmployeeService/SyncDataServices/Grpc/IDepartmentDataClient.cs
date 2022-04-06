using EmployeeService.Models;

namespace EmployeeService.SyncDataServices.Grpc;

public interface IDepartmentDataClient
{
    IEnumerable<Department> GetDepartments();
}