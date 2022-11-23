using CaaS.Core.Base;

namespace CaaS.Test.Common; 

public class StaticDateTimeOffsetProvider : IDateTimeOffsetProvider {
    private readonly DateTimeOffset _now;

    public StaticDateTimeOffsetProvider(DateTimeOffset now) {
        _now = now;
    }

    public DateTimeOffset GetNow() => _now;
}