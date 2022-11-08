namespace Yodeller.Application.Ports;

public interface IDiskSpaceEnvironmentCheck
{
    ValueTask<float> GetDiskSpacePercentage();
}