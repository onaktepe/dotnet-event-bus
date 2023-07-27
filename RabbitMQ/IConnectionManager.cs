namespace DotNetBB.EventBus.RabbitMQ;

public interface IConnectionManager
{
    IRabbitMQConnection GetPublisherConnection();
    IRabbitMQConnection GetConsumerConnection();
}