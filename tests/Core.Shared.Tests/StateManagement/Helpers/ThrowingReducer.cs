using Core.Shared.StateManagement;

namespace Core.Shared.Tests.StateManagement.Helpers;

internal class ThrowingReducer : IStateReducer<TestState>
{
    public TestState Invoke(TestState oldState)
    {
        throw new InvalidOperationException("Test reducer failure.");
    }
}
