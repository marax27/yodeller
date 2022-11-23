using Yodeller.Application.Features.GetAllRequests;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingGetAllRequestsReducer
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
    public async Task GivenEmptyStateReturnEmptyCollection()
    {
        var tcs = new TaskCompletionSource<GetAllRequestsQuery.Result>();
        var givenState = new DownloadRequestsState(new());
        var sut = new GetAllRequestsReducer(tcs.SetResult);

        var newState = sut.Invoke(givenState);
        var result = await tcs.Task;

        result.Requests.Should().BeEmpty();
    }

    [Fact]
    public async Task GivenMultipleRequestsInStateThenReturnExpectedRequests()
    {
        var tcs = new TaskCompletionSource<GetAllRequestsQuery.Result>();
        var givenState = new DownloadRequestsState(new List<DownloadRequest>()
        {
            _sampleRequest with { Id = "id1", MediaLocator = "ML1", Status = DownloadRequestStatus.New },
            _sampleRequest with { Id = "id2", MediaLocator = "ML2", Status = DownloadRequestStatus.InProgress },
        });
        var sut = new GetAllRequestsReducer(tcs.SetResult);

        var newState = sut.Invoke(givenState);
        var result = await tcs.Task;

        result.Requests.Should().BeEquivalentTo(new GetAllRequestsQuery.DownloadRequestDto[]
        {
            new("id1", "ML1", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), "New"),
            new("id2", "ML2", false, Array.Empty<GetAllRequestsQuery.HistoryEntryDto>(), "In progress")
        });
    }
}
