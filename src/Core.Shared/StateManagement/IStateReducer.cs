namespace Core.Shared.StateManagement;

/// <summary>
/// Reducer is an operation that can be performed on a state.
/// </summary>
/// <typeparam name="TState"></typeparam>
public interface IStateReducer<TState>
{
    TState Invoke(TState oldState);
}
