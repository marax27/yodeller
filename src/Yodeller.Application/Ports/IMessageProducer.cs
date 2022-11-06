using Yodeller.Application.Models;

namespace Yodeller.Application.Ports;

public interface IMessageProducer
{
    void Produce(DownloadRequest request);
}
