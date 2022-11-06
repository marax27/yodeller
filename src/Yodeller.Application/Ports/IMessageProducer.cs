namespace Yodeller.Application.Ports;

public interface IMessageProducer<in TBaseMessage> where TBaseMessage : class
{
    void Produce(TBaseMessage message);
}
