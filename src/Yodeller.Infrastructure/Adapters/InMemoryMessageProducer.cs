using System.Collections.Concurrent;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class InMemoryMessageProducer : IMessageProducer<BaseMessage>
{
    private readonly ConcurrentBag<BaseMessage> _requests = new();

    public void Produce(BaseMessage message) => _requests.Add(message);
}
