using MediatR;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Features;

public record EnvironmentChecksQuery : IRequest<EnvironmentChecksQuery.Result>
{
    public record Result(
        float AvailableDiskSpacePercentage,
        bool DownloaderAvailable,
        bool PostProcessingAvailable
    );
}

public class EnvironmentChecksQueryHandler : IRequestHandler<EnvironmentChecksQuery, EnvironmentChecksQuery.Result>
{
    private readonly IDiskSpaceEnvironmentCheck _diskSpaceCheck;
    private readonly IApplicationAvailableEnvironmentCheck _applicationCheck;

    public EnvironmentChecksQueryHandler(IDiskSpaceEnvironmentCheck diskSpaceCheck, IApplicationAvailableEnvironmentCheck applicationCheck)
    {
        _diskSpaceCheck = diskSpaceCheck ?? throw new ArgumentNullException(nameof(diskSpaceCheck));
        _applicationCheck = applicationCheck ?? throw new ArgumentNullException(nameof(applicationCheck));
    }

    public async Task<EnvironmentChecksQuery.Result> Handle(EnvironmentChecksQuery request, CancellationToken cancellationToken)
    {
        var diskTask = _diskSpaceCheck.GetDiskSpacePercentage();
        var downloaderTask = _applicationCheck.IsAvailable("yt-dlp");
        var postProcessTask = _applicationCheck.IsAvailable("ffmpeg");

        var diskPercentage = await diskTask;
        var downloaderResult = await downloaderTask;
        var postProcessResult = await postProcessTask;

        return new(diskPercentage, downloaderResult, postProcessResult);
    }
}
