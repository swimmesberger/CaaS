namespace CaaS.Core.Base; 

public interface IDateTimeOffsetProvider {
    DateTimeOffset GetNow();
}

public static class DateTimeOffsetProvider {
    public static IDateTimeOffsetProvider Instance = new DefaultDateTimeOffsetProvider();
    public static DateTimeOffset GetNow() => Instance.GetNow();
}

public class DefaultDateTimeOffsetProvider : IDateTimeOffsetProvider {
    public DateTimeOffset GetNow() => DateTimeOffset.UtcNow;
}