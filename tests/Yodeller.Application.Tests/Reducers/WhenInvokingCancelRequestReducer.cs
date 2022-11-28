using Yodeller.Application.Features.CancelRequest;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Reducers;

public class WhenInvokingCancelRequestReducer
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
    public void GivenNewRequestToCancelThenRequestIsCancelled()
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "0001", Status = DownloadRequestStatus.New }
        }, new());
        var sut = new CancelRequestReducer("0001");

        var newState = sut.Invoke(givenState);

        newState.Requests.Single().Status
            .Should().Be(DownloadRequestStatus.Cancelled);
    }

    [Theory]
    [InlineData(DownloadRequestStatus.Failed)]
    [InlineData(DownloadRequestStatus.Completed)]
    [InlineData(DownloadRequestStatus.InProgress)]
    [InlineData(DownloadRequestStatus.Cancelled)]
    public void GivenRequestToCancelIsNotNewThenDoNotUpdateStatus(DownloadRequestStatus givenRequestStatus)
    {
        var givenState = new DownloadRequestsState(new()
        {
            _sampleRequest with { Id = "0002", Status = givenRequestStatus }
        }, new());
        var sut = new CancelRequestReducer("0002");

        var newState = sut.Invoke(givenState);

        newState.Requests.Single().Status
            .Should().Be(givenRequestStatus);
    }
}
