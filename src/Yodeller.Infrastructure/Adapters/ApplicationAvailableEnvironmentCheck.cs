﻿using Yodeller.Application.Ports;
using Yodeller.Infrastructure.Utilities;

namespace Yodeller.Infrastructure.Adapters;

public class ApplicationAvailableEnvironmentCheck : IApplicationAvailableEnvironmentCheck
{
    public async ValueTask<bool> IsAvailable(string applicationName)
    {
        var execution = new ProcessExecution();

        var result = await execution.Run(
            new(
                applicationName,
                "--help",
                SuppressSubProcessOutput,
                SuppressSubProcessOutput,
                null
            )
        );

        return result.ExitCode == 0;
    }

    private void SuppressSubProcessOutput(string line)
    {

    }
}
