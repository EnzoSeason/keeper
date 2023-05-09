namespace Reporting.API.Utils;

public interface IClock
{
    DateTime Now { get; }
}

public class Clock: IClock
{
    public DateTime Now => DateTime.UtcNow;
}