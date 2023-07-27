namespace DotNetBB.EventBus.RabbitMQ;

public class PublisherConfig
{
    private const string _defaultExchangeName = "DotNetBBExchange";
    private string? _providedExchangeName;
    private bool _createProvidedExchange = false;

    internal string ExhangeName => _providedExchangeName ?? _defaultExchangeName;
    internal bool IsDefaultExchangeUsing => ExhangeName.Equals(_defaultExchangeName);
    internal bool CreateExchange => IsDefaultExchangeUsing ? _createProvidedExchange : true;

    public PublisherConfig SetExchangeName(string exchangeName)
    {
        _providedExchangeName = exchangeName;
        return this;
    }

    public PublisherConfig CreateExhangeIfNotExist()
    {
        _createProvidedExchange = true;
        return this;
    }

}