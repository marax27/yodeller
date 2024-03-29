﻿using Core.Shared.StateManagement;
using Yodeller.Application.Features.RequestDownload;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;
using Yodeller.Application.Tests.Helpers;

namespace Yodeller.Application.Tests.Features;

public class WhenRequesting2Downloads
{
    private readonly RequestDownloadCommand _firstCommand = new(Array.Empty<string>(), "http://example-media-page.com?id=123", true);
    private readonly RequestDownloadCommand _otherCommand = new(Array.Empty<string>(), "http://videos.abcd/sample", false);

    private readonly DateTime _firstRequestTime = new(2022, 11, 15, 23, 59, 57);
    private readonly DateTime _otherRequestTime = new(2022, 11, 16, 0, 0, 1);

    private readonly Mock<IMessageProducer<IStateReducer<DownloadRequestsState>>> _producerMock = new();

    private readonly StubClock _stubClock;

    public WhenRequesting2Downloads()
    {
        _stubClock = new(new(new[] { _firstRequestTime, _otherRequestTime, }));
    }

    [Fact]
    public async Task Given2ValidCommandsThenProduceExactly2Reducers()
    {
        var sut = new RequestDownloadCommandHandler(_producerMock.Object, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        _producerMock.Verify(mock => mock.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Given2ValidCommandsThenProduceExpectedValues()
    {
        var stubProducer = new StubReducerProducer();
        var sut = new RequestDownloadCommandHandler(stubProducer, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        var actualFirst = stubProducer.GetRegisteredDownloadRequests().First();
        actualFirst.MediaLocator.Should().Be(_firstCommand.MediaLocator);
        actualFirst.RequestedTime.Should().Be(_firstRequestTime);
        actualFirst.AudioOnly.Should().Be(true);
        actualFirst.Status.Should().Be(DownloadRequestStatus.New);

        var actualOther = stubProducer.GetRegisteredDownloadRequests().Last();
        actualOther.MediaLocator.Should().Be(_otherCommand.MediaLocator);
        actualOther.RequestedTime.Should().Be(_otherRequestTime);
        actualOther.AudioOnly.Should().Be(false);
        actualOther.Status.Should().Be(DownloadRequestStatus.New);
    }

    [Fact]
    public async Task Given2ValidCommandsThenReducersShouldHaveDifferentIds()
    {
        var stubProducer = new StubReducerProducer();
        var sut = new RequestDownloadCommandHandler(stubProducer, _stubClock);

        await sut.Handle(_firstCommand, CancellationToken.None);
        await sut.Handle(_otherCommand, CancellationToken.None);

        var actualIds = stubProducer.GetRegisteredDownloadRequests()
            .Select(request => request.Id)
            .ToArray();
        actualIds.Should().OnlyHaveUniqueItems();
    }
}
