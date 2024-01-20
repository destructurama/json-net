using Serilog.Events;

namespace Destructurama.JsonNet.Tests.Support;

public static class Extensions
{
    public static object? LiteralValue(this LogEventPropertyValue @this)
    {
        return ((ScalarValue)@this).Value;
    }
}
