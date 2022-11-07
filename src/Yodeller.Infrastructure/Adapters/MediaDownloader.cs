using System.Diagnostics;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class YtDlpMediaDownloader : IMediaDownloader
{
    public bool Download(DownloadProcessSpecification what)
    {
        var arguments = what.AudioOnly
            ? $"-f \"bestaudio/best\" -ciw -v --extract-audio --audio-quality 0 --audio-format mp3 {what.MediaLocator}"
            : what.MediaLocator;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false,
                WorkingDirectory = "/out"
            }
        };

        process.Start();
        
        process.WaitForExit();

        return process.ExitCode == 0;
    }
}
