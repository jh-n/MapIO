using System;
using System.Text;

namespace MapIO.Core.Extensions
{
    public static class EncodingExtensions
    {
#if NETSTANDARD
        public static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            return encoding.GetString(bytes.ToArray(), 0, bytes.Length);
        }
#endif
    }
}
