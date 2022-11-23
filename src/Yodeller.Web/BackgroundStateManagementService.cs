using Core.Shared.StateManagement;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Web;

public class BackgroundStateManagementService : BackgroundService
{
    private readonly IStateManager<DownloadRequestsState> _stateManager;

    public BackgroundStateManagementService(IMessageConsumer<IStateReducer<DownloadRequestsState>> messageConsumer)
    {
        var initialState = new DownloadRequestsState(new());
        _stateManager = new DownloadRequestsStateManager(initialState, messageConsumer);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _stateManager.Update();
            await Task.Yield();
        }
    }
}
