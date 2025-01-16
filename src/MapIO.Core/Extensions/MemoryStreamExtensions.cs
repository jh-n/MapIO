using System;
using System.IO;

namespace MapIO.Core.Extensions
{
    public static class MemoryStreamExtensions
    {
#if NETSTANDARD
        public static int Read(this MemoryStream stream, Span<byte> buffer)
        {
            var bytes = new byte[buffer.Length];
            var count = stream.Read(bytes, 0, bytes.Length);
            bytes.CopyTo(buffer);
            return count;
        }

        public static void Write(this MemoryStream stream, ReadOnlySpan<byte> buffer)
        {
            stream.Write(buffer.ToArray(), 0, buffer.Length);
        }
#endif
    }
}
