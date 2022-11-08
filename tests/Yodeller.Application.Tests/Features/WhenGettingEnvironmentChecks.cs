using Yodeller.Application.Features;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Tests.Features;

public class WhenGettingEnvironmentChecks
{
    [Theory]
    [InlineData(100f, true, true)]
    [InlineData(100f, false, true)]
    [InlineData(0f, true, false)]
    [InlineData(15f, false, false)]
    public async Task GivenIndividualCheckResultsThenReturnExpectedValues(
        float givenAvailableDiskSpace, bool givenDownloaderAvailable, bool givenPostProcessingAvailable)
    {
        var mockDiskSpaceCheck = new Mock<IDiskSpaceEnvironmentCheck>();
        mockDiskSpaceCheck.Setup(mock => mock.GetDiskSpacePercentage())
            .Returns(ValueTask.FromResult(givenAvailableDiskSpace));

        var mockApplicationCheck = new Mock<IApplicationAvailableEnvironmentCheck>();
        mockApplicationCheck.Setup(mock => mock.IsAvailable("yt-dlp"))
            .Returns(ValueTask.FromResult(givenDownloaderAvailable));
        mockApplicationCheck.Setup(mock => mock.IsAvailable("ffmpeg"))
            .Returns(ValueTask.FromResult(givenPostProcessingAvailable));

        var sut = new EnvironmentChecksQueryHandler(mockDiskSpaceCheck.Object, mockApplicationCheck.Object);

        var result = await sut.Handle(new(), CancellationToken.None);

        result.Should().BeEquivalentTo(
            new EnvironmentChecksQuery.Result(
                givenAvailableDiskSpace,
                givenDownloaderAvailable,
                givenPostProcessingAvailable
            )
        );
    }
}
