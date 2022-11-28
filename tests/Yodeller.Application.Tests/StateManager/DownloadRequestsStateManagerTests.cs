using Core.Shared.StateManagement;
using Microsoft.Extensions.Logging;
using Yodeller.Application.Downloader.Reducers;
using Yodeller.Application.Features.RequestDownload;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.StateManager;

public class DownloadRequestsStateManagerTests
{
    private static readonly DateTime SampleDateTime = new(2001, 10, 10, 12, 30, 45);
    private readonly DownloadRequest _sampleRequest = new("id1", SampleDateTime, "ml1", false, new List<string>(), new List<HistoryEntry>(), DownloadRequestStatus.New);

    private readonly Mock<IMessageConsumer<IStateReducer<DownloadRequestsState>>> _consumerMock = new();
    private readonly Mock<IClock> _clockMock = new();
    private readonly Mock<ILogger<DownloadRequestsStateManager>> _loggerMock = new();
    private readonly Mock<IUserNotificationsHub> _hubMock = new();

    [Fact]
    public async Task GivenRequestLockedWhenUpdatingStateThenSend1UserNotification()
    {
        var sut = CreateSut();
        sut.Dispatch(new AddNewRequestReducer(_sampleRequest));
        sut.Dispatch(new LockRequestForDownloadReducer(_ => {}));

        await sut.Update();

        _hubMock.Verify(mock =>
            mock.SendRequestUpdate(It.IsAny<IReadOnlyCollection<DownloadRequest>>()), Times.Once);
    }

    [Fact]
    public async Task GivenMultipleUpdatesAfterRequestLockedThenSend1UserNotification()
    {
        var sut = CreateSut();
        sut.Dispatch(new AddNewRequestReducer(_sampleRequest));
        sut.Dispatch(new LockRequestForDownloadReducer(_ => { }));
        sut.Dispatch(new AddNewRequestReducer(_sampleRequest with { Id = "id2" }));

        await sut.Update();

        _hubMock.Verify(mock =>
            mock.SendRequestUpdate(It.IsAny<IReadOnlyCollection<DownloadRequest>>()), Times.Once);
    }

    [Fact]
    public async Task GivenNoReducersThenDoNotSendUserNotifications()
    {
        var sut = CreateSut();

        await sut.Update();

        _hubMock.Verify(mock =>
            mock.SendRequestUpdate(It.IsAny<IReadOnlyCollection<DownloadRequest>>()), Times.Never);
    }

    [Fact]
    public async Task GivenNoRelevantReducersThenDoNotSendUserNotifications()
    {
        var sut = CreateSut();
        sut.Dispatch(new AddNewRequestReducer(_sampleRequest));

        await sut.Update();

        _hubMock.Verify(mock =>
            mock.SendRequestUpdate(It.IsAny<IReadOnlyCollection<DownloadRequest>>()), Times.Never);
    }

    private DownloadRequestsStateManager CreateSut()
    {
        DownloadRequestsState state = new(new(), new());
        return new(
            state,
            _consumerMock.Object,
            _clockMock.Object,
            _loggerMock.Object,
            _hubMock.Object
        );
    }
}
