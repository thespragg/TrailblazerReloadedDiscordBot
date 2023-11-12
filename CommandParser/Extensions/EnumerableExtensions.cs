using CommandParser.Models;

namespace CommandParser.Extensions;

public static class EnumerableExtensions
{
    public static SafeList<T> ToSafeList<T>(this IEnumerable<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new SafeList<T>(source);
    }
}