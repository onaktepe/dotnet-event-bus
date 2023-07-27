namespace DotNetBB.EventBus.RabbitMQ;

public class RabbitMQConnection : IRabbitMQConnection, IDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly ILogger _logger;
    private IConnection? _connection = null;
    public bool Disposed;

    public RabbitMQConnection(ConnectionFactory connectionFactory, ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public bool IsConnected => _connection == null ? false : _connection.IsOpen;

    public IModel CreateModel()
    {
        var connection = _connection ?? throw new Exception("Connection is null");
        if(!IsConnected)
        {
            throw new Exception("Connection is not exist nor open");
        }
        return connection.CreateModel();
    }

    public bool TryConnect()
    {
        try
        {
            _connection = _connectionFactory.CreateConnection();
        }
        catch (Exception ex)
        {
            _logger.LogCritical("RabbitMQ connection failed, ex-->", ex);
        }

        if(_connection == null || !IsConnected) 
        {
            _logger.LogCritical("RabbitMQ connection could not be created and opened");

            return false;        
        }

        _connection.ConnectionShutdown += OnConnectionShutdown;
        _connection.CallbackException += OnCallbackException;
        _connection.ConnectionBlocked += OnConnectionBlocked;

        _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);
        
        return true;
    }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        
        if(_connection == null) return;
        
        _connection.ConnectionShutdown -= OnConnectionShutdown;
        _connection.CallbackException -= OnCallbackException;
        _connection.ConnectionBlocked -= OnConnectionBlocked;
        _connection.Dispose();
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
         if (Disposed) return;

        _logger.LogWarning("A RabbitMQ connection is blocked. Trying to re-connect...");

        TryConnect();
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
         if (Disposed) return;

        _logger.LogWarning("A RabbitMQ callback exception is occured. Trying to re-connect...");

        TryConnect();
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
          if (Disposed) return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        TryConnect();
    }
}