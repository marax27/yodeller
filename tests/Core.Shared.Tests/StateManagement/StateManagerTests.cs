using Core.Shared.StateManagement;
using Core.Shared.Tests.StateManagement.Helpers;

namespace Core.Shared.Tests.StateManagement;

public class StateManagerTests
{
    private readonly TestState _givenSampleState = new(new() { 3, 3, -1 });

    [Fact]
    public void GivenNoReducersRunThenManagerContainsInitialState()
    {
        var sut = new StateManager<TestState>(_givenSampleState);

        // No operations.

        sut.State.Values.Should().Equal(3, 3, -1);
    }

    [Fact]
    public async Task GivenReducerThatAddsItemsThenManagerContainsExpectedState()
    {
        var sut = new StateManager<TestState>(_givenSampleState);
        sut.Dispatch(new AddValueReducer(9));

        await sut.Update();

        sut.State.Values.Should().Equal(3, 3, -1, 9);
    }

    [Fact]
    public async Task GivenReadReducerBetweenWriteReducersThenReadExpectedValue()
    {
        const int expectedSum = 3 + 3 - 1 + 12 + 24 + 36;
        var tcs = new TaskCompletionSource<int>();

        var sut = new StateManager<TestState>(_givenSampleState);
        sut.Dispatch(new AddValueReducer(12));
        sut.Dispatch(new AddValueReducer(24));
        sut.Dispatch(new AddValueReducer(36));
        sut.Dispatch(new GetSumOfValuesReducer(tcs.SetResult));
        sut.Dispatch(new AddValueReducer(48));
        sut.Dispatch(new AddValueReducer(60));

        await sut.Update();

        var actualSum = await tcs.Task;
        actualSum.Should().Be(expectedSum);
    }

    [Fact]
    public async Task GivenReducerThatThrowsThenThrowIdenticalException()
    {
        var sut = new StateManager<TestState>(_givenSampleState);
        sut.Dispatch(new ThrowingReducer());
        
        var act = async () => await sut.Update();
        
        var thrownException = (await act.Should().ThrowExactlyAsync<ReduceException>()).Which;
        thrownException.Message.Should().Be("Failed to reduce 'ThrowingReducer'.");

        thrownException.InnerException.Should().BeOfType<InvalidOperationException>();
        thrownException.InnerException!.Message.Should().Be("Test reducer failure.");
    }
}
