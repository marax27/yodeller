using FluentAssertions;
using Moq;
using Yodeller.Application.Features;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.Tests.Helpers;

namespace Yodeller.Application.Tests.Features;

public class WhenRequesting2Downloads
{
    private readonly RequestDownloadCommand _firstCommand = new("http://example-media-page.com?id=123");
    private readonly RequestDownloadCommand _otherCommand = new("http://videos.abcd/sample");

    private readonly DateTime _firstRequestTime = new(2022, 11, 15, 23, 59, 57);
    private readonly DateTime _otherRequestTime = new(2022, 11, 16, 0, 0, 1);

    private readonly Mock<IRequestRepository> _requestRepositoryMock = new();

    private readonly StubClock _stubClock;

    public WhenRequesting2Downloads()
    {
        _stubClock = new(new(new[] { _firstRequestTime, _otherRequestTime, }));
    }

    [Fact]
    public async Task Given2ValidCommandsThenRepositoryReceivedExactly2Requests()
    {
        var sut = new RequestDownloadCommandHandler(_requestRepositoryMock.Object, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        _requestRepositoryMock.Verify(mock => mock.Add(It.IsAny<DownloadRequest>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Given2ValidCommandsThenRepositoryReceivedExpectedValues()
    {
        var stubRepository = new StubRequestRepository();
        var sut = new RequestDownloadCommandHandler(stubRepository, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        var actualFirst = stubRepository.GetRegisteredDownloadRequests().First();
        actualFirst.MediaLocator.Should().Be(_firstCommand.MediaLocator);
        actualFirst.RequestedTime.Should().Be(_firstRequestTime);
        actualFirst.Status.Should().Be(DownloadRequestStatus.New);

        var actualOther = stubRepository.GetRegisteredDownloadRequests().Last();
        actualOther.MediaLocator.Should().Be(_otherCommand.MediaLocator);
        actualOther.RequestedTime.Should().Be(_otherRequestTime);
        actualOther.Status.Should().Be(DownloadRequestStatus.New);
    }

    [Fact]
    public async Task Given2ValidCommandsThenRequestsShouldHaveDifferentIds()
    {
        var stubRepository = new StubRequestRepository();
        var sut = new RequestDownloadCommandHandler(stubRepository, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        var actualIds = stubRepository.GetRegisteredDownloadRequests()
            .Select(request => request.Id)
            .ToArray();
        actualIds.Should().OnlyHaveUniqueItems();
    }
}
