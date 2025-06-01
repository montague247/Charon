using System.Security.Cryptography;
using System.Text;

namespace Charon;

public static class TypeExtensions
{
    public static string? TypeName(this Type type, bool withoutVersion = true)
    {
        if (type == null)
            return default;

        var fullName = type.FullName!;

        if (withoutVersion)
        {
            int startIdx;

            while ((startIdx = fullName.IndexOf(", Version=", StringComparison.Ordinal)) != -1)
            {
                var endIdx = fullName.IndexOf(", PublicKeyToken=", StringComparison.Ordinal) + 17;
                var pkt = fullName.Substring(endIdx, 4);

                if (string.Compare(pkt, "null", StringComparison.Ordinal) == 0)
                    endIdx += 4;
                else
                    endIdx += 16;

                fullName = string.Concat(fullName[..startIdx], fullName[endIdx..]);
            }
        }

        return string.Concat(fullName, ", ", type.Assembly.GetName().Name);
    }
}
