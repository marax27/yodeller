using Core.Shared.StateManagement;
using Yodeller.Application.Features.ClearFinished;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Features;

public class WhenClearingFinishedRequests
{
    private readonly Mock<IMessageProducer<IStateReducer<DownloadRequestsState>>> _producerMock = new();

    [Fact]
    public async Task GivenSampleRequestThenProduceExactlyOneReducer()
    {
        var sut = new ClearFinishedCommandHandler(_producerMock.Object);
        var givenCommand = new ClearFinishedCommand();

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()), Times.Once);
    }

    [Fact]
    public async Task GivenSampleRequestThenProduceExpectedReducer()
    {
        var sut = new ClearFinishedCommandHandler(_producerMock.Object);
        var expectedReducer = new ClearFinishedRequestsReducer();
        var givenCommand = new ClearFinishedCommand();

        await sut.Handle(givenCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(expectedReducer), Times.Once);
    }
}
