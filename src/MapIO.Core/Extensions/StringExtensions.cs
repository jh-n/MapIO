using System.Text;

namespace MapIO.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Repeat(this string text, int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++) sb.Append(text);
            return sb.ToString();
        }

        public static string Repeat(this char ch, int count)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < count; i++) sb.Append(ch);
            return sb.ToString();
        }
    }
}