using Microsoft.Extensions.Logging;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Downloader;

public class MediaDownloaderJob
{
    private readonly IMessageConsumer<BaseMessage> _messageConsumer;
    private readonly IDownloadRequestsRepository _requestsRepository;
    private readonly ILogger<MediaDownloaderJob> _logger;

    public MediaDownloaderJob(
        IMessageConsumer<BaseMessage> messageConsumer,
        IDownloadRequestsRepository requestsRepository,
        ILogger<MediaDownloaderJob> logger)
    {
        _messageConsumer = messageConsumer ?? throw new ArgumentNullException(nameof(messageConsumer));
        _requestsRepository = requestsRepository ?? throw new ArgumentNullException(nameof(requestsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Execute()
    {
        ConsumeAllMessages();

        // TODO download.
    }

    private void ConsumeAllMessages()
    {
        var processedMessageCount = 0;

        while (_messageConsumer.TryConsume(out var message))
        {
            _logger.LogInformation("Evaluating a message: {MessageType}...", message.GetType().Name);
            message.Invoke(_requestsRepository);
            ++processedMessageCount;
        }

        if (processedMessageCount > 0)
        {
            _logger.LogInformation("Evaluated {MessageCount} message(s).", processedMessageCount);
        }
    }
}
