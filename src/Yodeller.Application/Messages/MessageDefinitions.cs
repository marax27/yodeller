using Yodeller.Application.Models;

namespace Yodeller.Application.Messages;

public abstract record BaseMessage;

public record RequestedNewDownload(DownloadRequest Request) : BaseMessage;
