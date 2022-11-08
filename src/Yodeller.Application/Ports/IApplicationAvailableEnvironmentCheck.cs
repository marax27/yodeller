namespace Yodeller.Application.Ports;

public interface IApplicationAvailableEnvironmentCheck
{
    ValueTask<bool> IsAvailable(string applicationName);
}