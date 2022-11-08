using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class DumbDiskSpaceEnvironmentCheck : IDiskSpaceEnvironmentCheck
{
    public ValueTask<float> GetDiskSpacePercentage()
    {
        return ValueTask.FromResult(100f);
    }
}