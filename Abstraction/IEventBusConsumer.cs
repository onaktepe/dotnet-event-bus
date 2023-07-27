namespace DotNetBB.EventBus.Abstraction;

public interface IEventBusConsumer
{
    void Subscribe<TEvent, THandler>()
        where TEvent : BusEvent
        where THandler : IBusEventHandler<TEvent>;

    void SubscribeToQueue<TEvent, THandler>(string queueName)
        where TEvent : BusEvent
        where THandler : IBusEventHandler<TEvent>;

    void Unsubscribe<TEvent, THandler>()
        where TEvent: BusEvent
        where THandler: IBusEventHandler<TEvent>;
}