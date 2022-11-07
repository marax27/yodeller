using System.Diagnostics;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class YtDlpMediaDownloader : IMediaDownloader
{
    public bool Download(string mediaLocator)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "yt-dlp",
                Arguments = mediaLocator,
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
