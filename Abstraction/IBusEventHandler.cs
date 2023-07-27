namespace DotNetBB.EventBus.Abstraction;

public interface IBusEventHandler<T>
    where T: BusEvent
{
    void Handle(T busEvent);
}