using System.Collections.Concurrent;

namespace Core.Shared.StateManagement;

public class StateManager<TState> : IStateManager<TState>
{
    private readonly ConcurrentQueue<IStateReducer<TState>> _reducerQueue = new();

    public TState State { get; private set; }

    public StateManager(TState initialState)
    {
        State = initialState;
    }

    public void Dispatch(IStateReducer<TState> reducer)
    {
        _reducerQueue.Enqueue(reducer);
    }

    public virtual async ValueTask Update()
    {
        while (_reducerQueue.TryDequeue(out var reducer))
        {
            await OnReduce(reducer);
        }
    }

    protected virtual ValueTask OnReduce(IStateReducer<TState> reducer)
    {
        try
        {
            State = reducer.Invoke(State);
            return ValueTask.CompletedTask;
        }
        catch (Exception e)
        {
            throw new ReduceException($"Failed to reduce '{reducer.GetType().Name}'.", e);
        }
    }
}
