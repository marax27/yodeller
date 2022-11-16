using Yodeller.Application.Messages;
using static Yodeller.Application.Models.DownloadRequestStatus;

namespace Yodeller.Application.Tests.Repository;

public class WhenInvokingRequestedDownloadCancellationMessage
{
    private const string NewRequestId = "1110";
    private const string InProgressRequestId = "1111";
    private const string CancelledRequestId = "1112";
    private const string FailedRequestId = "1113";
    private const string NonexistentRequestId = "9999";

    private readonly DownloadRequestsRepository _sut = new();

    public WhenInvokingRequestedDownloadCancellationMessage()
    {
        var sampleTime = new DateTime(2001, 10, 30, 7, 45, 0);

        _sut.Add(new(NewRequestId, sampleTime, $"http://video.page/{NewRequestId}", false, New));
        _sut.Add(new(InProgressRequestId, sampleTime, $"http://video.page/{InProgressRequestId}", false, InProgress));
        _sut.Add(new(CancelledRequestId, sampleTime, $"http://video.page/{CancelledRequestId}", false, Cancelled));
        _sut.Add(new(FailedRequestId, sampleTime, $"http://video.page/{FailedRequestId}", false, Failed));
    }

    [Fact]
    public void GivenRequestToCancelIsNewThenRequestIsCancelled()
    {
        new RequestedDownloadCancellation(NewRequestId).Invoke(_sut);

        _sut.FindById(NewRequestId).Status.Should().Be(Cancelled);
    }

    [Fact]
    public void GivenRequestToCancelDoesNotExistThenThrow()
    {
        var act = () => new RequestedDownloadCancellation(NonexistentRequestId).Invoke(_sut);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void GivenRequestToCancelIsInProgressThenDoNothing()
    {
        new RequestedDownloadCancellation(InProgressRequestId).Invoke(_sut);

        _sut.FindById(InProgressRequestId).Status.Should().Be(InProgress);
    }

    [Fact]
    public void GivenRequestToCancelIsCancelledThenDoNothing()
    {
        new RequestedDownloadCancellation(CancelledRequestId).Invoke(_sut);

        _sut.FindById(CancelledRequestId).Status.Should().Be(Cancelled);
    }

    [Fact]
    public void GivenRequestToCancelIsFailedThenDoNothing()
    {
        new RequestedDownloadCancellation(FailedRequestId).Invoke(_sut);

        _sut.FindById(FailedRequestId).Status.Should().Be(Failed);
    }
}
