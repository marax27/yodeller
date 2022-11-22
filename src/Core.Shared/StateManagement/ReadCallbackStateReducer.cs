namespace Core.Shared.StateManagement;

/// <summary>
/// A special type of reducer that doesn't modify the state. Rather, it calculates a result
/// based on the state, and passes the result through the callback.
/// </summary>
/// <typeparam name="TState"></typeparam>
/// <typeparam name="TResult"></typeparam>
public abstract class ReadCallbackStateReducer<TState, TResult> : IStateReducer<TState>
{
    private readonly Action<TResult> _callback;

    protected ReadCallbackStateReducer(Action<TResult> callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public TState Invoke(TState oldState)
    {
        var result = Compute(oldState);
        _callback.Invoke(result);
        return oldState;
    }

    protected abstract TResult Compute(TState state);
}
