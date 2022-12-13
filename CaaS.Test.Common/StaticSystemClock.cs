using CaaS.Core.Base;

namespace CaaS.Test.Common; 

public class StaticSystemClock : ISystemClock {
    public DateTimeOffset UtcNow { get; }
    public StaticSystemClock(DateTimeOffset now) {
        UtcNow = now;
    }
}