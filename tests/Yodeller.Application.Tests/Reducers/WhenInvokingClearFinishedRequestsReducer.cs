using Yodeller.Application.Features.ClearFinished;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingClearFinishedRequestsReducer
{
    private static readonly DateTime SampleDateTime = new(2005, 1, 1, 3, 45, 59);

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
    public void GivenEmptyStateThenReturnEmptyState()
    {
        var givenState = new DownloadRequestsState(new());
        var sut = new ClearFinishedRequestsReducer();

        var newState = sut.Invoke(givenState);

        newState.Should().BeEquivalentTo(givenState);
    }

    [Fact]
    public void GivenMultipleRequestsInStateThenRemoveExpectedRequests()
    {
        var givenState = new DownloadRequestsState(new List<DownloadRequest>()
        {
            _sampleRequest with { Id = "0001", Status = DownloadRequestStatus.New },
            _sampleRequest with { Id = "0002", Status = DownloadRequestStatus.InProgress },
            _sampleRequest with { Id = "0003", Status = DownloadRequestStatus.Completed },
            _sampleRequest with { Id = "0004", Status = DownloadRequestStatus.Failed },
            _sampleRequest with { Id = "0005", Status = DownloadRequestStatus.Cancelled },
        });
        var sut = new ClearFinishedRequestsReducer();

        var newState = sut.Invoke(givenState);

        newState.Requests.Should().BeEquivalentTo(new[]
        {
            _sampleRequest with { Id = "0001", Status = DownloadRequestStatus.New },
            _sampleRequest with { Id = "0002", Status = DownloadRequestStatus.InProgress },
        });
    }
}
