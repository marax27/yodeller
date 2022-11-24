using Core.Shared.StateManagement;
using Microsoft.Extensions.Logging;
using Yodeller.Application.Ports;

namespace Yodeller.Application.State;

public class DownloadRequestsStateManager : StateManager<DownloadRequestsState>
{
    private readonly IMessageConsumer<IStateReducer<DownloadRequestsState>> _messageConsumer;
    private readonly IClock _clock;
    private readonly ILogger<DownloadRequestsStateManager> _logger;

    public DownloadRequestsStateManager(DownloadRequestsState initialState, IMessageConsumer<IStateReducer<DownloadRequestsState>> messageConsumer, IClock clock, ILogger<DownloadRequestsStateManager> logger)
        : base(initialState)
    {
        _messageConsumer = messageConsumer;
        _clock = clock;
        _logger = logger;
    }

    public override void Update()
    {
        while (_messageConsumer.TryConsume(out var reducer))
            Dispatch(reducer);

        base.Update();
    }

    protected override void OnReduce(IStateReducer<DownloadRequestsState> reducer)
    {
        var reducerType = reducer.GetType().Name;

        var startTime = _clock.GetNow();
        _logger.LogDebug("Reduce '{ReducerType}' start at {StartTime}", startTime, reducerType);

        try
        {
            base.OnReduce(reducer);
        }
        catch (ReduceException ex)
        {
            _logger.LogError(ex, "Failed to reduce: {ErrorMessage}", ex.Message);
            throw;
        }

        var endTime = _clock.GetNow();
        _logger.LogDebug("Reduce '{ReducerType}' end at {StartTime}", startTime, reducerType);
    }
}
