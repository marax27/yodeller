using FluentAssertions;
using Moq;
using Yodeller.Application.Features;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.Tests.Helpers;

namespace Yodeller.Application.Tests.Features;

public class WhenRequestingDownload
{
    private readonly DateTime _sampleRequestTime = new(2022, 11, 01, 15, 40, 51);

    private readonly RequestDownloadCommand _sampleCommand = new("http://example-media-page.com?id=123");

    private readonly Mock<IRequestRepository> _requestRepositoryMock = new();

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
        var sut = new RequestDownloadCommandHandler(_requestRepositoryMock.Object, _stubClock);

        var act = async () => await sut.Handle(_sampleCommand, CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task GivenValidCommandThenRepositoryReceivedExactlyOneRequest()
    {
        var sut = new RequestDownloadCommandHandler(_requestRepositoryMock.Object, _stubClock);

        await sut.Handle(_sampleCommand, CancellationToken.None);

        _requestRepositoryMock.Verify(mock => mock.Add(It.IsAny<DownloadRequest>()), Times.Once);
    }

    [Fact]
    public async Task GivenValidCommandThenRepositoryReceivedExpectedRequest()
    {
        var stubRepository = new StubRequestRepository();
        var sut = new RequestDownloadCommandHandler(stubRepository, _stubClock);

        await sut.Handle(_sampleCommand, CancellationToken.None);

        var actualRequest = stubRepository.GetRegisteredDownloadRequests().Single();

        actualRequest.Should().NotBeNull();
        actualRequest.Id.Should().NotBeNullOrWhiteSpace();
        actualRequest.MediaLocator.Should().Be(_sampleCommand.MediaLocator);
        actualRequest.RequestedTime.Should().Be(_sampleRequestTime);
        actualRequest.Status.Should().Be(DownloadRequestStatus.New);
    }
}
