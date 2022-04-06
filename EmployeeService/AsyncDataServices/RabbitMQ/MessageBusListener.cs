using System.Text;
using EmployeeService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmployeeService.AsyncDataServices;

public class MessageBusListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection = null!;
    private IModel _channel = null!;
    private string _queue = null!;

    public MessageBusListener(IConfiguration configuration,
        IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        RmqConnect();
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (_channel == null)
        {
            Console.WriteLine(">> RabbitMQ connection is not open");
            return Task.CompletedTask;
        }

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ModuleHandle, ea) =>
        {
            Console.WriteLine(">> Event received");
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            _eventProcessor.ProcessEvent(message);
        };

        _channel.BasicConsume(queue: _queue,
            autoAck: true,
            consumer: consumer);

        return Task.CompletedTask;
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
            _queue = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queue,
                exchange: _configuration["Exchange"],
                routingKey: string.Empty);

            Console.WriteLine(">> RabbitMQ listener is up");
        }
        catch (Exception e)
        {
            Console.WriteLine($">> RabbitMQ connection failed: {e.Message}");
        }
    }

    private void RmqConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine(">> RabbitMQ connection shutdown");
    }

    public override void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_channel != null && _channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
        Console.WriteLine(">> RabbitMQ listener disposed");
    }
}