using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class DumbApplicationAvailableEnvironmentCheck : IApplicationAvailableEnvironmentCheck
{
    public ValueTask<bool> IsAvailable(string applicationName)
    {
        return ValueTask.FromResult(true);
    }
}