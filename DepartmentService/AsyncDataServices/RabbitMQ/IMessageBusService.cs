using DepartmentService.Models;

namespace DepartmentService.AsyncDataClient.RabbitMQ;

public interface IMessageBusService
{
    void PublishDepartment(DepartmentPublishDto departmentPublishDto);
}