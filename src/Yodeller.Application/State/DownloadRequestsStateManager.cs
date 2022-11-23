using Core.Shared.StateManagement;
using Yodeller.Application.Ports;

namespace Yodeller.Application.State;

public class DownloadRequestsStateManager : StateManager<DownloadRequestsState>
{
    private readonly IMessageConsumer<IStateReducer<DownloadRequestsState>> _messageConsumer;

    public DownloadRequestsStateManager(DownloadRequestsState initialState, IMessageConsumer<IStateReducer<DownloadRequestsState>> messageConsumer)
        : base(initialState)
    {
        _messageConsumer = messageConsumer;
    }

    public override void Update()
    {
        while (_messageConsumer.TryConsume(out var reducer))
            Dispatch(reducer);

        base.Update();
    }
}
