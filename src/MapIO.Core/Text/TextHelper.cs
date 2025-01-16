using MapIO.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MapIO.Core.Text
{
    public static class TextHelper
    {
        /// <summary>
        /// *No trim
        /// </summary>
        /// <param name="line"></param>
        /// <param name="innerSeperators"></param>
        /// <param name="outerSeperators"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> ReadLineAsKeyValuePairs(this string line, char[] innerSeperators, char[] outerSeperators = null)
        {
            var rawKVs = outerSeperators.IsNullOrEmpty()
                ? Enumerable.Empty<string>().Append(line)
#if NETSTANDARD
                : line.Split(outerSeperators, StringSplitOptions.RemoveEmptyEntries).WhereNot(string.IsNullOrWhiteSpace);
#else
                : line.Split(outerSeperators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
#endif
            return rawKVs.Select(rawKV =>
#if NETSTANDARD
                rawKV.Split(innerSeperators, 2).Select(s => s.Trim()).ToArray()
#else
                rawKV.Split(innerSeperators, 2, StringSplitOptions.TrimEntries)
#endif
                )
                .Where(kv => kv.Length == 2)
                .Select(kv => new KeyValuePair<string, string>(kv[0], kv[1])).ToList();
        }

        public static List<KeyValuePair<string, string>> ReadLineAsKeyValuePairs(this string line, char innerSeperator, char? outerSeperator = null)
            => line.ReadLineAsKeyValuePairs(new char[] { innerSeperator }, outerSeperator.HasValue ? new char[] { outerSeperator.Value } : null);
    }
}