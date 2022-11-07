namespace Yodeller.Application.Ports;

public interface IMessageConsumer<TBaseMessage> where TBaseMessage : class
{
    bool TryConsume(out TBaseMessage message);
}