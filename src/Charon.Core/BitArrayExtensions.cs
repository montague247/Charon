using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace Charon;

public static class BitArrayExtensions
{
    public static string? ToString(this BitArray? value, int size)
    {
        if (value == null)
            return default;

        var sb = new StringBuilder();

        for (int i = 0; i < size * size; i++)
        {
            sb.Append(value[i] ? '1' : '0');
        }

        return sb.ToString();
    }
}
