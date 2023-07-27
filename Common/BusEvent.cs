namespace DotNetBB.EventBus.Common;

public record BusEvent
{
    [JsonInclude]
    public Guid Id {get; private init;}

    [JsonInclude]
    public DateTime CreatedDate {get; private init;}

    public BusEvent() 
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    [JsonConstructor]
    public BusEvent(Guid id, DateTime createdDate)
    {
        Id = id;
        CreatedDate = createdDate;
    }
}