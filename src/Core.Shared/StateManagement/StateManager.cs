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

    public virtual void Update()
    {
        while (_reducerQueue.TryDequeue(out var reducer))
        {
            OnReduce(reducer);
        }
    }

    protected virtual void OnReduce(IStateReducer<TState> reducer)
    {
        try
        {
            State = reducer.Invoke(State);
        }
        catch (Exception e)
        {
            throw new ReduceException($"Failed to reduce '{reducer.GetType().Name}'.", e);
        }
    }
}
