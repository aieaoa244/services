using System.Text;
using System.Text.Json;
using DepartmentService.Models;
using RabbitMQ.Client;

namespace DepartmentService.AsyncDataClient.RabbitMQ;

public class RmqMessageBusService : IMessageBusService
{
    private readonly IConfiguration _configuration;
    private IConnection _connection = null!;
    private IModel _channel = null!;

    public RmqMessageBusService(IConfiguration configuration)
    {
        _configuration = configuration;
        RmqConnect();
    }

    public void PublishDepartment(DepartmentPublishDto departmentPublishDto)
    {
        var message = JsonSerializer.Serialize(departmentPublishDto);
        if (_connection.IsOpen)
        {
            Console.WriteLine(">> Sending message");
            SendMessage(message);
        }
        else
        {
            Console.WriteLine(">> RabbitMQ connection is not open");
        }
    }

    private void SendMessage(string message)
    {
        var messageBody = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: _configuration["Exchange"],
            routingKey: string.Empty,
            // basicProperties: null,
            body: messageBody);
        Console.WriteLine($">> Sent message {message}");
    }

    private void RmqConnect()
    {
        var rmqFactory = new ConnectionFactory()
        {
            HostName = _configuration["RmqHost"],
            Port = int.Parse(_configuration["RmqPort"])
        };
        try
        {
            _connection = rmqFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: _configuration["Exchange"], type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += RmqConnectionShutdown;
            Console.WriteLine(">> RabbitMQ connection is up");
        }
        catch (Exception e)
        {
            Console.WriteLine($">> Message bus connection failed: {e.Message}");
        }
    }

    private void RmqConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine(">> RabbitMq connection shutdown");
    }

    public void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
            Console.WriteLine(">> Rmq service disposed");
        }
    }
}