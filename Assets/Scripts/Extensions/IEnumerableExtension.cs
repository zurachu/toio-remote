using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtension
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> ts)
    {
        return ts.Select((t, i) => (t, i));
    }
}
