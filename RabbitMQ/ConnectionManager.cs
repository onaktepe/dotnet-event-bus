namespace DotNetBB.EventBus.RabbitMQ;

public class ConnectionManager : IConnectionManager
{
    private readonly ConnectionFactory _connectionFactory;
    private IRabbitMQConnection? _consumerConnection;
    private IRabbitMQConnection? _publisherConnection;
    private readonly ILogger _logger;

    public ConnectionManager(ConnectionFactory connectionFactory, ILogger<ConnectionManager> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public IRabbitMQConnection GetConsumerConnection()
    {
        if(_consumerConnection == null)
        {
            _consumerConnection = new RabbitMQConnection(_connectionFactory, _logger);
        }
        if(!_consumerConnection.IsConnected)
        {
            _consumerConnection.TryConnect();
        }

        return _consumerConnection;
    }

    public IRabbitMQConnection GetPublisherConnection()
    {
        if(_publisherConnection == null)
        {
            _publisherConnection = new RabbitMQConnection(_connectionFactory, _logger);
        }
        if(!_publisherConnection.IsConnected)
        {
            _publisherConnection.TryConnect();
        }

        return _publisherConnection;
    }
}