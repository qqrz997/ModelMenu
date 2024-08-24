using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelMenu.Utilities.Extensions;

internal static class LinqExtensions
{
    public static void ForEach<TSource>(this IEnumerable<TSource> seq, Action<TSource> action)
    {
        foreach (var item in seq) action(item);
    }

    public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> seq, Func<TSource, bool> predicate) =>
        seq.Where(v => !predicate(v));

    public static IEnumerable<TSource> DistinctWhere<TSource, TKey>(this IEnumerable<TSource> seq, Func<TSource, TKey> predicate, IEqualityComparer<TKey> comparer = null)
    {
        var knownKeys = new HashSet<TKey>(comparer);
        return seq.Where(item => knownKeys.Add(predicate(item)));
    }
}
