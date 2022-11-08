using Microsoft.Extensions.Logging;
using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class DiskSpaceEnvironmentCheck : IDiskSpaceEnvironmentCheck
{
    private readonly ILogger<DiskSpaceEnvironmentCheck> _logger;

    public DiskSpaceEnvironmentCheck(ILogger<DiskSpaceEnvironmentCheck> logger)
    {
        _logger = logger;
    }

    public ValueTask<float> GetDiskSpacePercentage()
    {
        var workingDirectory = "/out";

        var drive = new DriveInfo(workingDirectory);

        _logger.LogInformation("Disk space check: drive = {DriveLabel}, total free = {TotalFree}, total = {Total}",
            drive.VolumeLabel, drive.TotalFreeSpace, drive.TotalSize);

        var percentage = MathF.Round(drive.TotalFreeSpace * 100f / drive.TotalSize);

        return ValueTask.FromResult(percentage);
    }
}