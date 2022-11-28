namespace Core.Shared.StateManagement;

/// <summary>
/// Manages access to a `TState` data structure. Allows multiple threads to safely
/// read from and write to the state.
/// </summary>
public interface IStateManager<TState>
{
    /// <summary>
    /// Queues the reducer for future processing.
    /// Reducer will be evaluated during state manager's next update.
    /// </summary>
    /// <param name="reducer"></param>
    void Dispatch(IStateReducer<TState> reducer);

    /// <summary>
    /// Evaluate all reducers dispatched so far (and not evaluated by a previous update).
    /// </summary>
    ValueTask Update();
}
