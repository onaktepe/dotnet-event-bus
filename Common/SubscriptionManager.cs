namespace DotNetBB.EventBus.Common;

public class SubscriptionManager: ISubscriptionManager
{
    private Dictionary<string, List<SubscriptionInfo>> _subscriptions = new Dictionary<string, List<SubscriptionInfo>>();

    public SubscriptionInfo AddSubscription<TEvent,THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>
    {
        var eventName = GetEventName<TEvent>();

        var eventSubscriptions = GetSubscriptions(eventName); 
        if(eventSubscriptions == null)
        {
            eventSubscriptions = new List<SubscriptionInfo>();
            _subscriptions.Add(eventName, eventSubscriptions);
        }

        var subscription = GetSubscription<TEvent, THandler>();
        if(subscription == null) 
        {
            subscription = new SubscriptionInfo(typeof(TEvent), typeof(THandler));
            eventSubscriptions.Add(subscription);
        }
        
        return subscription;
    }

    public void RemoveSubscriptions<TEvent>()
        where TEvent: BusEvent
    {
        var eventName = GetEventName<TEvent>();
        _subscriptions.Remove(eventName);
    }

    public void RemoveSubscriptions(string eventName)
    {
        _subscriptions.Remove(eventName);
    }

    public void RemoveSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>
    {
        var subscriptions = GetSubscriptions<TEvent>();
        if(subscriptions == null) return;

        var subscription = FindSubscription<THandler>(subscriptions);
        if(subscription == null) return;

        subscriptions.Remove(subscription);

        return;
    }

    public void RemoveSubscription(SubscriptionInfo subscriptionInfo)
    {
        if(subscriptionInfo == null) return;

        var subscriptions = GetSubscriptions(subscriptionInfo.EventType);
        if(subscriptions == null) return;

        var subscription = FindSubscription(subscriptions, subscriptionInfo.HandlerType);
        if(subscription == null) return;

        subscriptions.Remove(subscription);

        return;
    }

    public List<SubscriptionInfo> GetSubscriptions<TEvent>()
        where TEvent: BusEvent
    {
        var eventName = GetEventName<TEvent>();
        return GetSubscriptions(eventName);
    }

    public List<SubscriptionInfo> GetSubscriptions(Type eventType)
    {
        return GetSubscriptions(GetEventName(eventType));
    }

    public List<SubscriptionInfo> GetSubscriptions(string eventName)
    {
         return _subscriptions.ContainsKey(eventName) ? _subscriptions[eventName] : null;
    }

    public SubscriptionInfo? GetSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>
    {
         var subscriptions = GetSubscriptions<TEvent>();
         if(subscriptions == null || subscriptions.Count == 0) return null;
         return FindSubscription<THandler>(subscriptions);
    }

    public bool HasSubscriptions<TEvent>()
        where TEvent: BusEvent
    {
         var subscriptions = GetSubscriptions<TEvent>();
         return subscriptions == null ? false : subscriptions.Count > 0;
    }

    public bool HasSubscriptions(string eventName)
    {
         var subscriptions = GetSubscriptions(eventName);
         return subscriptions == null ? false : subscriptions.Count > 0;
    }

    public bool HasSubscription<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>
    {
        var subscripton = GetSubscription<TEvent, THandler>();
        return subscripton != null;
    }

    public string GetEventName<TEvent>()
    {
        return GetEventName(typeof(TEvent));
    }

    private string GetEventName(Type eventType)
    {
        return eventType.Name;
    }

    private SubscriptionInfo? FindSubscription(List<SubscriptionInfo> subscriptions, Type handlerType)
    {
        if(subscriptions == null) return null;
        return subscriptions.Where(p=> p.HandlerType == handlerType).SingleOrDefault();
    }

    private SubscriptionInfo? FindSubscription<THandler>(List<SubscriptionInfo> subscriptions)
    {
        return FindSubscription(subscriptions, typeof(THandler));
    }

}