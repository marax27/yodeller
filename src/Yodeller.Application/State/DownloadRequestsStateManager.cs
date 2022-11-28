using Core.Shared.StateManagement;
using Microsoft.Extensions.Logging;
using Yodeller.Application.Ports;

namespace Yodeller.Application.State;

public class DownloadRequestsStateManager : StateManager<DownloadRequestsState>
{
    private readonly IMessageConsumer<IStateReducer<DownloadRequestsState>> _messageConsumer;
    private readonly IClock _clock;
    private readonly ILogger<DownloadRequestsStateManager> _logger;
    private readonly IUserNotificationsHub _userNotificationsHub;

    public DownloadRequestsStateManager(DownloadRequestsState initialState, IMessageConsumer<IStateReducer<DownloadRequestsState>> messageConsumer, IClock clock, ILogger<DownloadRequestsStateManager> logger, IUserNotificationsHub userNotificationsHub)
        : base(initialState)
    {
        _messageConsumer = messageConsumer;
        _clock = clock;
        _logger = logger;
        _userNotificationsHub = userNotificationsHub;
    }

    public override ValueTask Update()
    {
        while (_messageConsumer.TryConsume(out var reducer))
            Dispatch(reducer);

        return base.Update();
    }

    protected override async ValueTask OnReduce(IStateReducer<DownloadRequestsState> reducer)
    {
        var reducerType = reducer.GetType().Name;

        var startTime = _clock.GetNow();
        _logger.LogDebug("Reduce '{ReducerType}' start at {StartTime}", startTime, reducerType);

        try
        {
            await base.OnReduce(reducer);
        }
        catch (ReduceException ex)
        {
            _logger.LogError(ex, "Failed to reduce: {ErrorMessage}", ex.Message);
            throw;
        }

        try
        {
            await HandlePostReduceNotifications();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while processing post-reduce notifications: {ErrorMessage}", ex.Message);
            throw;
        }

        var endTime = _clock.GetNow();
        _logger.LogDebug("Reduce '{ReducerType}' end at {StartTime}", endTime, reducerType);
    }

    private async ValueTask HandlePostReduceNotifications()
    {
        if (!State.AlteredRequestIds.Any())
            return;

        var requests = State.Requests
            .Where(request => State.AlteredRequestIds.Contains(request.Id))
            .ToArray();

        State.AlteredRequestIds.Clear();

        await _userNotificationsHub.SendRequestUpdate(requests);
    }
}
