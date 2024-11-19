using System.Security.Cryptography;
using System.Text;

namespace Charon
{
    public static class StringExtensions
    {
        public static string? SingleLine(this string? value)
        {
            if (string.IsNullOrEmpty(value) ||
                !HasCharsToEscape(value))
                return value;

            var sb = new StringBuilder();

            foreach (var c in value)
            {
                switch (c)
                {
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '"':
                        sb.Append('\"');
                        break;
                    case '\0':
                        sb.Append("\\0");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        public static bool HasCharsToEscape(this string? value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            foreach (var c in value)
            {
                if (c == '\n' ||
                    c == '\r' ||
                    c == '\t' ||
                    c == '\\' ||
                    c == '\0' ||
                    c == '"')
                    return true;
            }

            return false;
        }

        public static string? ToEscapedString(this string? value, bool singleLine = true)
        {
            if (value == null)
                return value;

            if (singleLine)
                return string.Concat('"', SingleLine(value), '"');

            if (value.HasCharsToEscape())
            {
                var escapedStr = value.Replace("\"", "\"\"");

                return $"@\"{escapedStr}\"";
            }
            else
            {
                var escapedStr = value.Replace("\"", "\\\"");

                return $"\"{escapedStr}\"";
            }
        }

        public static Guid CreateGuid(this string value) => new(MD5.HashData(Encoding.UTF8.GetBytes(value)));
    }
}
