using System.Text.Json;
using DepartmentService.Models;
using EmployeeService.Models;

namespace EmployeeService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.Department_created:
                await DepartmentProcessor.CreateDepartment(_scopeFactory, message);
                break;
            case EventType.Department_updated:
                await DepartmentProcessor.UpdateDepartment(_scopeFactory, message);
                break;
            default:
                Console.WriteLine($">> Unknown event: {eventType}");
                break;
        }
    }

    private static EventType DetermineEvent(string message)
    {
        var eventType = JsonSerializer.Deserialize<GenericEvent>(message);
        if (eventType == null)
        {
            Console.WriteLine(">> Message contains no event type");
            throw new Exception(nameof(eventType));
        }

        return eventType.Event switch
        {
            "Department_created" => EventType.Department_created,
            "Department_updated" => EventType.Department_updated,
            _ => EventType.Undetermined,
        };
    }
}