using Yodeller.Application.Downloader.Reducers;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingLockForDownloadReducer
{
    private static readonly DateTime SampleDateTime = new(2022, 11, 15, 23, 59, 57);

    private readonly DownloadRequest _sampleRequest = new(
        "id1",
        SampleDateTime,
        "media-locator",
        false,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        DownloadRequestStatus.New
    );

    [Fact]
    public async Task GivenEmptyStateReturnNull()
    {
        var givenState = new DownloadRequestsState(new(), new());

        var result = await Act(givenState);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenMultipleNonEligibleRequestsReturnNull()
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "id1", MediaLocator = "ML1", Status = DownloadRequestStatus.Cancelled },
            _sampleRequest with { Id = "id2", MediaLocator = "ML2", Status = DownloadRequestStatus.Failed },
            _sampleRequest with { Id = "id3", MediaLocator = "ML3", Status = DownloadRequestStatus.Completed }
        }, new());

        var result = await Act(givenState);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenMultipleEligibleRequestsReturnOldestEligibleRequest()
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "id1", MediaLocator = "ML1", RequestedTime = SampleDateTime.AddSeconds(60) },
            _sampleRequest with { Id = "id2", MediaLocator = "ML2", RequestedTime = SampleDateTime.AddSeconds(1) },
            _sampleRequest with { Id = "id3", MediaLocator = "ML3", RequestedTime = SampleDateTime.AddSeconds(30) }
        }, new());

        var result = await Act(givenState);

        result!.Id.Should().Be("id2");
    }

    private static async Task<DownloadRequest?> Act(DownloadRequestsState givenState)
    {
        var tcs = new TaskCompletionSource<DownloadRequest?>();
        var sut = new LockRequestForDownloadReducer(tcs.SetResult);

        sut.Invoke(givenState);
        return await tcs.Task;
    }
}
