namespace EmployeeService.EventProcessing;

public interface IEventProcessor
{
    public Task ProcessEvent(string message);
}