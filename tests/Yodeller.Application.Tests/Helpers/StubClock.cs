using Yodeller.Application.Ports;

namespace Yodeller.Application.Tests.Helpers;

internal class StubClock : IClock
{
    private readonly Queue<DateTime> _returnValues;

    public StubClock(Queue<DateTime> returnValues) => _returnValues = returnValues;

    public DateTime GetNow() => _returnValues.Dequeue();
}
