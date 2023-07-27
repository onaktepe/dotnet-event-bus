namespace DotNetBB.EventBus.RabbitMQ;

public class EventBusPublisher : IEventBusPublisher
{
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<EventBusPublisher> _logger;
    private IRabbitMQConnection Connection => _connectionManager.GetPublisherConnection();
    private IModel _channel;

    private readonly PublisherConfig _publisherConfig;

    public EventBusPublisher(IConnectionManager connectionManager, ILogger<EventBusPublisher> logger, Action<PublisherConfig>? config = null)// string brokerName)
    {
        _connectionManager = connectionManager;
        _logger = logger;
        _publisherConfig = new PublisherConfig();
        if(config != null)
        {
            config.Invoke(_publisherConfig);
        } 
        //_brokerName = brokerName;
        _channel = CreateChannel();
        
    }

    
    public void Publish(BusEvent busEvent)
    {
       var properties = _channel.CreateBasicProperties();
        properties.DeliveryMode = 2; // persistent

        //_logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

        var eventName = busEvent.GetType().Name;
        var body = JsonSerializer.SerializeToUtf8Bytes(busEvent, busEvent.GetType(), new JsonSerializerOptions{
            WriteIndented = true
        });

        _channel.BasicPublish(
            exchange: _publisherConfig.ExhangeName,
            routingKey: eventName,
            mandatory: true,
            basicProperties: properties,
            body: body);
    }

    private IModel CreateChannel()
    {
        var createdChannel = Connection.CreateModel();
        _logger.LogInformation($"Channel created, ready to publish exchange:{_publisherConfig.ExhangeName}");
        
        return createdChannel;
    }
}