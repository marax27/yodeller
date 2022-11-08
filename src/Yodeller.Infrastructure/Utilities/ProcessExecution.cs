using System.Diagnostics;

namespace Yodeller.Infrastructure.Utilities;

internal class ProcessExecution
{
    public record ProcessParameters(
        string ApplicationName,
        string Arguments,
        string? WorkingDirectory
    );

    public record Result(
        int ExitCode
    );

    public async ValueTask<Result> Run(ProcessParameters parameters)
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = parameters.ApplicationName,
                Arguments = parameters.Arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = false
            }
        };

        if (parameters.WorkingDirectory is not null)
        {
            process.StartInfo.WorkingDirectory = parameters.WorkingDirectory;
        }

        process.Start();

        await process.WaitForExitAsync();

        return new Result(process.ExitCode);
    }
}
