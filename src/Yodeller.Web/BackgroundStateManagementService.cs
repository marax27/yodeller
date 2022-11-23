using System.Diagnostics;
using Core.Shared.StateManagement;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Web;

public class BackgroundStateManagementService : BackgroundService
{
    private readonly IStateManager<DownloadRequestsState> _stateManager;
    private readonly ILogger<BackgroundStateManagementService> _logger;

    public BackgroundStateManagementService(IMessageConsumer<IStateReducer<DownloadRequestsState>> messageConsumer, ILogger<BackgroundStateManagementService> logger)
    {
        var initialState = new DownloadRequestsState(new());
        _stateManager = new DownloadRequestsStateManager(initialState, messageConsumer);
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting state manager loop in the background.");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _stateManager.Update();
                await Task.Delay(97, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unhandled error in the state manager loop: {ErrorMessage}", e.Message);
            throw;
        }
        _logger.LogInformation("State manager loop finished.");
    }
}
