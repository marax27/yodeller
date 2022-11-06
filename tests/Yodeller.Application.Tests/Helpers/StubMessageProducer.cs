using Yodeller.Application.Messages;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Tests.Helpers;

internal class StubMessageProducer : IMessageProducer<BaseMessage>
{
    private readonly List<BaseMessage> _downloadRequests = new();

    public IEnumerable<DownloadRequest> GetAll() => throw new NotImplementedException();

    public IReadOnlyList<DownloadRequest> GetRegisteredDownloadRequests() => _downloadRequests
        .OfType<RequestedNewDownload>()
        .Select(message => message.Request)
        .ToArray();

    public void Produce(BaseMessage message) => _downloadRequests.Add(message);
}
