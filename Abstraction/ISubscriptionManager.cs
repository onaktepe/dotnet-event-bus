namespace DotNetBB.EventBus.Abstraction;

public interface ISubscriptionManager
{
    SubscriptionInfo AddSubscription<TEvent,THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>;

    void RemoveSubscriptions<TEvent>()
        where TEvent: BusEvent;

    void RemoveSubscriptions(string eventName);
    
    void RemoveSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>;
   
    void RemoveSubscription(SubscriptionInfo subscriptionInfo);

    List<SubscriptionInfo> GetSubscriptions<TEvent>()
        where TEvent: BusEvent;

    List<SubscriptionInfo> GetSubscriptions(string eventName);
    
    SubscriptionInfo? GetSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>;

    bool HasSubscriptions<TEvent>()
        where TEvent: BusEvent;

    bool HasSubscriptions(string eventName);

    public bool HasSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>;

    string GetEventName<TEvent>();

}