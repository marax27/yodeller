﻿using System.Text;
using Microsoft.Extensions.Logging;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;
using Yodeller.Infrastructure.Utilities;

namespace Yodeller.Infrastructure.Adapters;

public class YtDlpMediaDownloader : IMediaDownloader
{
    private readonly ILogger<YtDlpMediaDownloader> _logger;

    public YtDlpMediaDownloader(ILogger<YtDlpMediaDownloader> logger)
    {
        _logger = logger;
    }

    public bool Download(DownloadProcessSpecification what)
    {
        var arguments = CreateDownloaderArguments(what);

        var execution = new ProcessExecution();

        var foundErrors = false;
        var errorOutputBuilder = new StringBuilder();

        var result = execution.Run(
            new(
                "yt-dlp",
                arguments,
                stdout => { },
                stderr =>
                {
                    if (!foundErrors)
                        _logger.LogError("Errors during download... Full log will be emitted after the sub-process exits.");
                    foundErrors = true;
                    errorOutputBuilder.AppendLine(stderr);
                },
                "/out"
            )
        ).Result;

        if (foundErrors)
            _logger.LogError("Downloader error output: {ErrorOutput}", errorOutputBuilder.ToString());

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
