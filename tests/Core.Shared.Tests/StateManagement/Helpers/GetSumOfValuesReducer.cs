using Core.Shared.StateManagement;

namespace Core.Shared.Tests.StateManagement.Helpers;

internal class GetSumOfValuesReducer : ReadCallbackStateReducer<TestState, int>
{
    public GetSumOfValuesReducer(Action<int> callback) : base(callback) { }

    protected override int Compute(TestState state)
    {
        return state.Values.Sum();
    }
}
