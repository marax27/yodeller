using Core.Shared.StateManagement;

namespace Core.Shared.Tests.StateManagement.Helpers;

internal record AddValueReducer(int Value) : IStateReducer<TestState>
{
    public TestState Invoke(TestState oldState)
    {
        return new TestState(oldState.Values.Append(Value).ToList());
    }
}
