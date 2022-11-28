using Yodeller.Application.Downloader.Reducers;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingFinishDownloadReducer
{
    private static readonly DateTime SampleDateTime = new(2022, 11, 15, 23, 59, 57);
    private static readonly DateTime OtherDateTime = new(2022, 11, 16, 1, 1, 1);

    private static readonly DownloadRequest _sampleRequest = new(
        "id1",
        SampleDateTime,
        "media-locator",
        false,
        Array.Empty<string>(),
        Array.Empty<HistoryEntry>(),
        DownloadRequestStatus.New
    );

    private readonly DownloadRequestsState _givenState = new DownloadRequestsState(new()
    {
        _sampleRequest with { Id = "id1", MediaLocator = "ML1", Status = DownloadRequestStatus.InProgress },
        _sampleRequest with { Id = "id2", MediaLocator = "ML2", Status = DownloadRequestStatus.Failed },
        _sampleRequest with { Id = "id3", MediaLocator = "ML3", Status = DownloadRequestStatus.Completed }
    }, new());

    [Fact]
    public void GivenRequestFinishedSuccessfullyThenStatusIsCompleted()
    {
        var sut = new FinishDownloadReducer("id1", true, OtherDateTime);

        var newState = sut.Invoke(_givenState);

        newState.Requests[0].Status.Should().Be(DownloadRequestStatus.Completed);
    }

    [Fact]
    public void GivenRequestFinishedSuccessfullyThenRequestHasExpectedHistoryEntry()
    {
        var sut = new FinishDownloadReducer("id1", true, OtherDateTime);

        var newState = sut.Invoke(_givenState);

        newState.Requests[0].History.Last()
            .Should().BeEquivalentTo(new HistoryEntry("Completed.", OtherDateTime));
    }

    [Fact]
    public void GivenRequestFinishedNotSuccessfullyThenStatusIsFailed()
    {
        var sut = new FinishDownloadReducer("id1", false, OtherDateTime);

        var newState = sut.Invoke(_givenState);

        newState.Requests[0].Status.Should().Be(DownloadRequestStatus.Failed);
    }

    [Fact]
    public void GivenRequestFinishedNotSuccessfullyThenRequestHasExpectedHistoryEntry()
    {
        var sut = new FinishDownloadReducer("id1", false, OtherDateTime);

        var newState = sut.Invoke(_givenState);

        newState.Requests[0].History.Last()
            .Should().BeEquivalentTo(new HistoryEntry("Failed.", OtherDateTime));
    }
}
