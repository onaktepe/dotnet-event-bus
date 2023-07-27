namespace DotNetBB.EventBus.RabbitMQ;

public class EventBusConsumer : IEventBusConsumer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnectionManager _connectionManager;
    private readonly ILogger<EventBusConsumer> _logger;
    private readonly ISubscriptionManager _subscriptionManager;
    private IRabbitMQConnection Connection => _connectionManager.GetConsumerConnection();
    private Dictionary<string, IModel> _channels;

    private readonly ConsumerConfig _consumerConfig;

    public EventBusConsumer(IServiceProvider serviceProvider, 
        IConnectionManager connectionManager,
        ILogger<EventBusConsumer> logger,
        ISubscriptionManager? subscriptionManager = null, 
        Action<ConsumerConfig>? config = null)
    {
        _serviceProvider = serviceProvider;
        _connectionManager = connectionManager;
        _logger = logger;
        _subscriptionManager = subscriptionManager ?? new SubscriptionManager();
        _consumerConfig = new ConsumerConfig();
        if(config != null)
        {
            config.Invoke(_consumerConfig);
        } 

        _channels = new Dictionary<string, IModel>();
    }

    public void Subscribe<TEvent, THandler>()
        where TEvent : BusEvent
        where THandler : IBusEventHandler<TEvent>
    {

        var hasSubscription = _subscriptionManager.HasSubscription<TEvent, THandler>();
        if(hasSubscription) return;

        _subscriptionManager.AddSubscription<TEvent, THandler>();

        var channel = ConfigureChannel(_consumerConfig.QueueName);
    }

    public void SubscribeToQueue<TEvent, THandler>(string queueName)
        where TEvent : BusEvent
        where THandler : IBusEventHandler<TEvent>
    {
        var hasSubscription = _subscriptionManager.HasSubscription<TEvent, THandler>();
        if(hasSubscription) return;

        _subscriptionManager.AddSubscription<TEvent, THandler>();

        var channel = ConfigureChannel(queueName);
    }

    public void Unsubscribe<TEvent, THandler>()
        where TEvent : BusEvent
        where THandler : IBusEventHandler<TEvent>
    {
        _subscriptionManager.RemoveSubscription<TEvent, THandler>();
    }

    private IModel ConfigureChannel(string queueName)
    {
        IModel channel;
        if(!_channels.ContainsKey(queueName))
        {
             channel = CreateChannel(queueName);
            _channels.Add(queueName, channel);
        }
        else 
        {
            channel = _channels[queueName];
        }
        return channel;
    }

    private IModel CreateChannel(string queueName)
    {
        var channel = Connection.CreateModel();

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += MessageReceived;
        channel.BasicConsume(queue: queueName,
                     autoAck: true,
                     consumer: consumer);

        return channel;
    }

    private async Task MessageReceived(object? sender, BasicDeliverEventArgs e)
    {
        var body = e.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message}");

        var eventName = e.RoutingKey;
        if(string.IsNullOrWhiteSpace(eventName)) throw new ArgumentNullException("eventName");
        if(!_subscriptionManager.HasSubscriptions(eventName)) 
        {
            Console.WriteLine($"No subscription found for event {eventName}");
            return;
        }

        var subscriptions = _subscriptionManager.GetSubscriptions(eventName);
        bool allFailed = true;
        foreach (var subscription in subscriptions)
        {
            try
            {
                await ProcessMessage(message, subscription);
                allFailed = false;
            }
            catch (System.Exception)
            {
                Console.WriteLine($"Message processing failed for event:{eventName}, handler:{subscription.HandlerType?.ToString()}, message:{message}");
            }
        }
    }

    private async Task ProcessMessage(string message, SubscriptionInfo subscription)
    {
        using var scope = _serviceProvider.CreateAsyncScope();
        
        var handler = scope.ServiceProvider.GetService(subscription.HandlerType);
        if (handler == null) return;

        var eventObject = JsonSerializer.Deserialize(message, subscription.EventType);
        var handlerType = typeof(IBusEventHandler<>).MakeGenericType(subscription.EventType);
        
        await (Task)handlerType.GetMethod("Handle").Invoke(handler, new object[] { eventObject });
    }
}