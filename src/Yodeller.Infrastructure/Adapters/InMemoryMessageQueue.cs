using System.Collections.Concurrent;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class InMemoryMessageQueue : IMessageProducer<BaseMessage>, IMessageConsumer<BaseMessage>
{
    private readonly ConcurrentQueue<BaseMessage> _requests = new();

    public void Produce(BaseMessage message)
    {
        if (message is null)
            throw new ArgumentNullException(nameof(message));
        _requests.Enqueue(message);
    }

    public bool TryConsume(out BaseMessage message)
    {
        var result = _requests.TryDequeue(out message!);
        return result;
    }
}
