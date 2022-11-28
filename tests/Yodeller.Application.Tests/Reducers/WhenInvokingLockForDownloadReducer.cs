using Yodeller.Application.Downloader.Reducers;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingLockForDownloadReducer
{
    private static readonly DateTime SampleDateTime = new(2022, 11, 15, 23, 59, 57);
    private static readonly DateTime GivenLockDateTime = new(2022, 11, 16, 0, 0, 1);

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
    public async Task GivenEmptyStateThenReturnNull()
    {
        var givenState = new DownloadRequestsState(new(), new());

        var result = await Act(givenState);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GivenMultipleNonEligibleRequestsThenReturnNull()
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
    public async Task GivenMultipleEligibleRequestsThenReturnOldestEligibleRequest()
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

    [Fact]
    public async Task GivenEligibleRequestThenRequestHasNewHistoryEntry()
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "id1", MediaLocator = "ML1", Status = DownloadRequestStatus.New },
        }, new());
        var initialHistoryEntryCount = givenState.Requests[0].History.Count;

        var finalState = await ActOnState(givenState);

        finalState.Requests[0].History.Should().HaveCount(1 + initialHistoryEntryCount);
    }

    [Fact]
    public async Task GivenEligibleRequestThenNewestHistoryEntryContainsExpectedValues()
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "id1", MediaLocator = "ML1", Status = DownloadRequestStatus.New },
        }, new());

        var finalState = await ActOnState(givenState);

        finalState.Requests[0].History.Last()
            .Should().BeEquivalentTo(new HistoryEntry("Download started.", GivenLockDateTime));
    }

    private static async Task<DownloadRequest?> Act(DownloadRequestsState givenState)
    {
        var tcs = new TaskCompletionSource<DownloadRequest?>();
        var sut = new LockRequestForDownloadReducer(GivenLockDateTime, tcs.SetResult);

        sut.Invoke(givenState);
        return await tcs.Task;
    }

    private static async Task<DownloadRequestsState> ActOnState(DownloadRequestsState givenState)
    {
        var tcs = new TaskCompletionSource<DownloadRequest?>();
        var sut = new LockRequestForDownloadReducer(GivenLockDateTime, tcs.SetResult);

        var result = sut.Invoke(givenState);
        await tcs.Task;
        return result;
    }
}
