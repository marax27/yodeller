using Core.Shared.StateManagement;
using Yodeller.Application.Features.RequestDownload;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;
using Yodeller.Application.Tests.Helpers;

namespace Yodeller.Application.Tests.Features;

public class WhenRequestingDownload
{
    private readonly DateTime _sampleRequestTime = new(2022, 11, 01, 15, 40, 51);

    private readonly RequestDownloadCommand _sampleCommand = new(Array.Empty<string>(), "http://example-media-page.com?id=123", false);

    private readonly Mock<IMessageProducer<IStateReducer<DownloadRequestsState>>> _producerMock = new();

    private readonly StubClock _stubClock;

    public WhenRequestingDownload()
    {
        _stubClock = new(new(new[]
        {
            _sampleRequestTime,
        }));
    }

    [Fact]
    public async Task GivenValidCommandThenFinishSuccessfully()
    {
        var sut = new RequestDownloadCommandHandler(_producerMock.Object, _stubClock);

        var act = async () => await sut.Handle(_sampleCommand, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GivenValidCommandThenProduceExactlyOneReducer()
    {
        var sut = new RequestDownloadCommandHandler(_producerMock.Object, _stubClock);

        await sut.Handle(_sampleCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidCommandThenProduceExpectedReducer()
    {
        var stubProducer = new StubReducerProducer();
        var sut = new RequestDownloadCommandHandler(stubProducer, _stubClock);

        await sut.Handle(_sampleCommand, CancellationToken.None);

        var actualRequest = stubProducer.GetRegisteredDownloadRequests().Single();

        actualRequest.Should().NotBeNull();
        actualRequest.Id.Should().NotBeNullOrWhiteSpace();
        actualRequest.MediaLocator.Should().Be(_sampleCommand.MediaLocator);
        actualRequest.RequestedTime.Should().Be(_sampleRequestTime);
        actualRequest.Status.Should().Be(DownloadRequestStatus.New);
    }
}
