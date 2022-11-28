using System.Diagnostics;
using CliWrap;
using CliWrap.Buffered;

namespace Yodeller.Infrastructure.Utilities;

internal class ProcessExecution
{
    public record ProcessParameters(
        string ApplicationName,
        string Arguments,
        Action<string> StdoutDelegate,
        Action<string> StderrDelegate,
        string? WorkingDirectory
    );

    public record Result(
        int ExitCode
    );

    public async ValueTask<Result> Run(ProcessParameters parameters)
    {
        var command = CreateCliCommand(parameters);

        var result = await command.ExecuteBufferedAsync();

        return new(result.ExitCode);
    }

    private static Command CreateCliCommand(ProcessParameters parameters)
    {
        var result = Cli.Wrap(parameters.ApplicationName)
            .WithArguments(parameters.Arguments)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(parameters.StdoutDelegate))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(parameters.StderrDelegate));

        if (parameters.WorkingDirectory is { })
            result = result.WithWorkingDirectory(parameters.WorkingDirectory);

        return result;
    }

    public async ValueTask<Result> RunObsolete(ProcessParameters parameters)
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
