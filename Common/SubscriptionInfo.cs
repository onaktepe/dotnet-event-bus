namespace DotNetBB.EventBus.Common;

public class SubscriptionInfo
{
    public Type EventType { get; }
    public Type HandlerType { get; }

    public SubscriptionInfo(Type eventType, Type handlerType)
    {
        EventType = eventType;
        HandlerType = handlerType;
    }
}