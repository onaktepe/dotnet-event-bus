namespace DotNetBB.EventBus.RabbitMQ;

public interface IRabbitMQConnection
{
    bool IsConnected { get; }

    bool TryConnect();

    IModel CreateModel();
}