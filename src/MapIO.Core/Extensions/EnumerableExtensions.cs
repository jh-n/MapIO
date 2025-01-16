using System;
using System.Collections.Generic;
using System.Linq;

namespace MapIO.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TSource>> Page<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than 0.");
            for (int i = 0; ; i += pageSize)
            {
                var current = source.Skip(i).Take(pageSize);
                if (current.Any()) yield return current;
                else yield break;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) { foreach (var item in source) action(item); }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in source) action(item, index++);
        }

        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan> selector)
        {
            TimeSpan sum = TimeSpan.Zero;
            foreach (var item in source) sum += selector(item);
            return sum;
        }
        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan?> selector)
        {
            TimeSpan sum = TimeSpan.Zero;
            TimeSpan? curr;
            foreach (var item in source) if ((curr = selector(item)).HasValue) sum += curr.Value;
            return sum;
        }

        public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> source) => source == null || !source.Any();

        public static IEnumerable<TSource> WhereNot<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => source.Where(x => !predicate(x));

#if NETSTANDARD
        public static IEnumerable<(TFirst First, TSecond Second)> Zip<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second) => first.Zip(second, (f, s) => (f, s));

        public static IEnumerable<(TFirst First, TSecond Second, TThird Third)> Zip<TFirst, TSecond, TThird>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third) => first.Zip(second).Zip(third, (fs, t) => (fs.First, fs.Second, t));

        public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => new HashSet<TSource>(source);
#endif
    }
}