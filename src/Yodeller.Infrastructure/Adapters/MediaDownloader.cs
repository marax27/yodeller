using System.Text;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;
using Yodeller.Infrastructure.Utilities;

namespace Yodeller.Infrastructure.Adapters;

public class YtDlpMediaDownloader : IMediaDownloader
{
    public bool Download(DownloadProcessSpecification what)
    {
        var arguments = CreateDownloaderArguments(what);

        var execution = new ProcessExecution();

        var result = execution.Run(new("yt-dlp", arguments, "/out")).Result;

        return result.ExitCode == 0;
    }

    private static string CreateDownloaderArguments(DownloadProcessSpecification what)
    {
        var argumentsBuilder = new StringBuilder();

        if (what.AudioOnly)
        {
            argumentsBuilder.Append("-f \"bestaudio/best\" -ciw -v --extract-audio --audio-quality 0 --audio-format mp3 ");
        }

        argumentsBuilder.Append(what.MediaLocator);

        if (what.SubtitlePatterns.Any())
        {
            var subtitleArgument = string.Join(",", what.SubtitlePatterns);
            argumentsBuilder.Append($" --write-subs --sub-langs \"{subtitleArgument}\"");
        }

        return argumentsBuilder.ToString();
    }
}
