using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;
using Yodeller.Infrastructure.Utilities;

namespace Yodeller.Infrastructure.Adapters;

public class YtDlpMediaDownloader : IMediaDownloader
{
    public bool Download(DownloadProcessSpecification what)
    {
        var arguments = what.AudioOnly
            ? $"-f \"bestaudio/best\" -ciw -v --extract-audio --audio-quality 0 --audio-format mp3 {what.MediaLocator}"
            : what.MediaLocator;

        var execution = new ProcessExecution();

        var result = execution.Run(new("yt-dlp", arguments, "/out")).Result;

        return result.ExitCode == 0;
    }
}
