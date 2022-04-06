using DepartmentService.Models;

namespace DepartmentService.SyncDataServices.Http;

public interface IDepartmentDataClient
{
    Task SendDepartmentToEmployee(DepartmentPublishDto department);
}