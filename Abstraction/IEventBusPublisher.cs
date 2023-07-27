namespace DotNetBB.EventBus.Abstraction;

public interface IEventBusPublisher
{
    void Publish(BusEvent busEvent);
}