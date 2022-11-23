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
            State = reducer.Invoke(State);
        }
    }
}
