namespace DotNetBB.EventBus.RabbitMQ;

public class ConsumerConfig
{
    private const string _defaultExchangeName = "DotNetBBExchange";
    private const string _defaultQueueName = "DotNetBBQueue";

    private string? _providedExchangeName;
    private string? _providedQueueName;

    internal string ExhangeName => _providedExchangeName ?? _defaultExchangeName;
    internal string QueueName => _providedExchangeName ?? _defaultQueueName;

    internal bool IsDefaultExchangeUsing => ExhangeName.Equals(_defaultExchangeName);
    internal bool IsDefaultQueueUsing => QueueName.Equals(_defaultQueueName);

    public ConsumerConfig SetExchangeName(string exchangeName)
    {
        _providedExchangeName = exchangeName;
        return this;
    }

    public ConsumerConfig SetQueueName(string queueName)
    {
        _providedQueueName = queueName;
        return this;
    }
}