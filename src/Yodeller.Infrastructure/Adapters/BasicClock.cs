using Yodeller.Application.Ports;

namespace Yodeller.Infrastructure.Adapters;

public class BasicClock : IClock
{
    public DateTime GetNow() => DateTime.Now;
}
