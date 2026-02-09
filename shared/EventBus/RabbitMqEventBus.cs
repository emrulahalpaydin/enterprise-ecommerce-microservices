using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservices.Shared.EventBus;

public sealed class RabbitMqEventBus : IEventBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _publishChannel;
    private readonly RabbitMqEventBusOptions _options;
    private readonly ILogger<RabbitMqEventBus> _logger;
    private readonly ConcurrentDictionary<string, IModel> _consumerChannels = new();

    public RabbitMqEventBus(IOptions<RabbitMqEventBusOptions> options, ILogger<RabbitMqEventBus> logger)
    {
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _publishChannel = _connection.CreateModel();
        _publishChannel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
    }

    public Task PublishAsync<T>(T @event) where T : notnull
    {
        var eventName = typeof(T).Name;
        var body = JsonSerializer.SerializeToUtf8Bytes(@event);

        var props = _publishChannel.CreateBasicProperties();
        props.Persistent = true;
        props.ContentType = "application/json";

        _publishChannel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: eventName,
            basicProperties: props,
            body: body);

        _logger.LogInformation("Published event {EventName}", eventName);
        return Task.CompletedTask;
    }

    public void Subscribe<T>(Func<T, Task> handler) where T : notnull
    {
        var eventName = typeof(T).Name;
        var queueName = $"{_options.ServiceName}.{eventName}";

        var channel = _consumerChannels.GetOrAdd(queueName, _ =>
        {
            var c = _connection.CreateModel();
            c.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true, autoDelete: false);
            c.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            c.QueueBind(queue: queueName, exchange: _options.ExchangeName, routingKey: eventName);
            c.BasicQos(0, 10, false);
            return c;
        });

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, ea) =>
        {
            try
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                var payload = JsonSerializer.Deserialize<T>(message);
                if (payload is null)
                {
                    _logger.LogWarning("Received null payload for {EventName}", eventName);
                    channel.BasicAck(ea.DeliveryTag, false);
                    return;
                }

                await handler(payload);
                channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling event {EventName}", eventName);
                channel.BasicNack(ea.DeliveryTag, false, requeue: true);
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Subscribed to {EventName} on queue {Queue}", eventName, queueName);
    }

    public void Dispose()
    {
        foreach (var c in _consumerChannels.Values) c.Dispose();
        _publishChannel.Dispose();
        _connection.Dispose();
    }
}

public sealed class RabbitMqEventBusOptions
{
    public string HostName { get; init; } = "rabbitmq";
    public int Port { get; init; } = 5672;
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string VirtualHost { get; init; } = "/";
    public string ExchangeName { get; init; } = "microservices.exchange";
    public string ServiceName { get; init; } = "service";
}
