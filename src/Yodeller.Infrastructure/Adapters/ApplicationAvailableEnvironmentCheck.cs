using System.Diagnostics;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class ApplicationAvailableEnvironmentCheck : IApplicationAvailableEnvironmentCheck
{
    public async ValueTask<bool> IsAvailable(string applicationName)
    {
        Process process = new Process()
        {
            StartInfo = new()
            {
                FileName = applicationName,
                Arguments = "--help",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false
            }
        };

        process.Start();
        await process.WaitForExitAsync();

        return process.ExitCode == 0;
    }
}