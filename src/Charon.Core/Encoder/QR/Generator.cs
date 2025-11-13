using System.Collections;
using System.Text;

namespace Charon.Encoder.QR;

public static class Generator
{
    private const string AlphaChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

    /// <summary>
    /// Generates a QR final raster as a BitArray (row-major). Returns BitArray and outputs size (width/height).
    /// bit == true -> black module, false -> white module.
    /// Supports Versions 1..10 (tables included). ECC level parameter selectable.
    /// </summary>
    public static BitArray Generate(string text, EccLevel eccLevel, out int size)
    {
        ArgumentNullException.ThrowIfNull(text);

        // Detect mode (choose best single mode for simplicity). Could be extended to mix modes.
        var mode = ChooseMode(text);

        // Find minimal version that fits
        int version = ChooseVersion(text, mode, eccLevel);

        // Build data bit stream
        BitBuffer dataBits = EncodeData(text, mode, version);

        // Terminator + pad to full data codewords
        var versionInfo = VersionTables.GetVersionInfo(version, eccLevel);
        int totalDataCodewords = versionInfo.TotalDataCodewords;
        PadToDataCodewords(dataBits, totalDataCodewords);

        // Convert to codewords
        byte[] dataCodewords = dataBits.ToBytes();

        // Split into blocks and generate ECC
        var blocks = ErrorCorrection.InterleaveAndGenerate(dataCodewords, versionInfo);

        // Build final codeword sequence (data+ecc interleaved)
        byte[] finalCodewords = ErrorCorrection.InterleaveBlocks(blocks);

        // Build module matrix and place patterns
        int matrixSize = 21 + (version - 1) * 4;
        bool?[,] matrix = new bool?[matrixSize, matrixSize]; // null=unset, true=black, false=white

        // place function patterns
        ModulePlacer.PlaceFunctionPatterns(matrix, version);

        // place data bits
        PlaceDataBits(matrix, finalCodewords);

        // choose mask with minimal penalty
        int bestPenalty = int.MaxValue;
        bool[,]? bestMatrix = null;

        for (int mask = 0; mask < 8; mask++)
        {
            bool[,] copy = ConvertNullableToBool(matrix); // copy of fixed modules with false for unset

            ApplyDataMask(copy, matrix, mask);

            FormatAndVersionPlacer.PlaceFormatAndVersion(copy, eccLevel, mask, version);

            int penalty = MaskEvaluator.CalculatePenalty(copy);

            if (bestMatrix == null ||
                penalty < bestPenalty)
            {
                bestPenalty = penalty;
                bestMatrix = copy;
            }
        }

        // bestMatrix now holds final modules
        size = matrixSize;
        BitArray result = new(size * size);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                result[y * size + x] = bestMatrix![y, x];
            }
        }

        return result;
    }

    // ---------------------------
    // Mode selection (simple heuristics)
    // ---------------------------
    private static Mode ChooseMode(string text)
    {
        // Kanji check: attempt shift_jis encode and verify pairs fall in Kanji ranges
        try
        {
            Encoding shiftJis = Encoding.GetEncoding("shift_jis");
            byte[] sj = shiftJis.GetBytes(text);
            bool allKanjiPairs = sj.Length % 2 == 0;

            if (allKanjiPairs)
            {
                for (int i = 0; i < sj.Length; i += 2)
                {
                    int v = (sj[i] & 0xFF) << 8 | (sj[i + 1] & 0xFF);
                    if (!((v >= 0x8140 && v <= 0x9FFC) || (v >= 0xE040 && v <= 0xEBBF)))
                    {
                        allKanjiPairs = false;
                        break;
                    }
                }

                if (allKanjiPairs)
                    return Mode.Kanji;
            }
        }
        catch { /* platform may not support shift_jis - ignore */ }

        bool numeric = text.All(c => c >= '0' && c <= '9');

        if (numeric)
            return Mode.Numeric;

        bool alnum = text.ToUpperInvariant().All(c => AlphaChars.Contains(c));

        if (alnum)
            return Mode.Alphanumeric;

        return Mode.Byte;
    }

    // ---------------------------
    // Version selection
    // ---------------------------
    private static int ChooseVersion(string text, Mode mode, EccLevel ecc)
    {
        // We try versions 1..10 (expandable)
        for (int v = 1; v <= 10; v++)
        {
            int capacity = VersionTables.GetCharCapacity(v, mode, ecc);
            int needed = 0;
            switch (mode)
            {
                case Mode.Numeric:
                    needed = (text.Length); // capacity counts characters
                    break;
                case Mode.Alphanumeric:
                    needed = text.Length;
                    break;
                case Mode.Kanji:
                    needed = text.Length; // Kanji count as character pairs in encoding logic
                    break;
                case Mode.Byte:
                    needed = Encoding.UTF8.GetBytes(text).Length;
                    break;
            }
            if (needed <= capacity) return v;
        }
        throw new ArgumentException("Input too long for implemented versions (1..10). Extend VersionTables for larger versions.");
    }

    private static BitBuffer EncodeData(string text, Mode mode, int version)
    {
        // Mode indicator
        int modeIndicator = mode switch
        {
            Mode.Numeric => 0b0001,
            Mode.Alphanumeric => 0b0010,
            Mode.Byte => 0b0100,
            Mode.Kanji => 0b1000,
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported mode"),
        };

        BitBuffer bb = new();
        bb.Put(modeIndicator, 4);

        // Character count indicator length depends on mode & version
        int cciBits = VersionTables.GetCharacterCountIndicatorBits(version, mode);
        // character count
        int count = mode switch
        {
            Mode.Byte => Encoding.UTF8.GetBytes(text).Length,
            _ => text.Length
        };
        bb.Put(count, cciBits);

        // Delegate data encoding to helper methods
        switch (mode)
        {
            case Mode.Numeric:
                EncodeNumericData(text, bb);
                break;
            case Mode.Alphanumeric:
                EncodeAlphanumericData(text, bb);
                break;
            case Mode.Byte:
                EncodeByteData(text, bb);
                break;
            case Mode.Kanji:
                EncodeKanjiData(text, bb);
                break;
        }

        return bb;
    }

    private static void EncodeNumericData(string text, BitBuffer bb)
    {
        int i = 0;
        while (i < text.Length)
        {
            int take = Math.Min(3, text.Length - i);
            string part = text.Substring(i, take);
            int val = int.Parse(part);
            if (take == 3) bb.Put(val, 10);
            else if (take == 2) bb.Put(val, 7);
            else bb.Put(val, 4);
            i += take;
        }
    }

    private static void EncodeAlphanumericData(string text, BitBuffer bb)
    {
        int i = 0;
        while (i < text.Length)
        {
            if (i + 1 < text.Length)
            {
                int val = AlphaChars.IndexOf(char.ToUpperInvariant(text[i])) * 45 + AlphaChars.IndexOf(char.ToUpperInvariant(text[i + 1]));
                bb.Put(val, 11);
                i += 2;
            }
            else
            {
                int val = AlphaChars.IndexOf(char.ToUpperInvariant(text[i]));
                bb.Put(val, 6);
                i++;
            }
        }
    }

    private static void EncodeByteData(string text, BitBuffer bb)
    {
        byte[] bs = Encoding.UTF8.GetBytes(text);
        foreach (byte b in bs) bb.Put(b, 8);
    }

    private static void EncodeKanjiData(string text, BitBuffer bb)
    {
        Encoding shiftJis = Encoding.GetEncoding("shift_jis");
        byte[] sj = shiftJis.GetBytes(text);
        for (int i = 0; i < sj.Length; i += 2)
        {
            int val = ((sj[i] & 0xFF) << 8) | (sj[i + 1] & 0xFF);
            int adjusted = (val >= 0x8140 && val <= 0x9FFC) ? val - 0x8140 : val - 0xC140;
            int msb = (adjusted >> 8) & 0xFF;
            int lsb = adjusted & 0xFF;
            int final = msb * 0xC0 + lsb;
            bb.Put(final, 13);
        }
    }

    // ---------------------------
    // Padding to data codewords
    // ---------------------------
    private static void PadToDataCodewords(BitBuffer bits, int totalDataCodewords)
    {
        int totalBitsCapacity = totalDataCodewords * 8;
        // Terminator up to 4 zeros
        int terminator = Math.Min(4, totalBitsCapacity - bits.Count);
        for (int i = 0; i < terminator; i++) bits.Put(0, 1);

        // byte-align
        while (bits.Count % 8 != 0) bits.Put(0, 1);

        // pad with 0xEC 0x11 alternately
        byte[] padBytes = new byte[] { 0xEC, 0x11 };
        int iPad = 0;
        byte[] current = bits.ToBytes();
        int currentUsed = current.Length;
        while (currentUsed < totalDataCodewords)
        {
            // append pad byte bits
            for (int b = 7; b >= 0; b--)
            {
                bits.Put(((padBytes[iPad] >> b) & 1), 1);
            }
            iPad = (iPad + 1) % 2;
            current = bits.ToBytes();
            currentUsed = current.Length;
        }
    }

    // ---------------------------
    // Place data bits into matrix
    // ---------------------------
    private static void PlaceDataBits(bool?[,] matrix, byte[] finalCodewords)
    {
        int size = matrix.GetLength(0);
        List<bool> bits = ExtractBits(finalCodewords);

        int bitIndex = 0;
        int x = size - 1;
        int dir = -1; // up=-1, down=+1

        while (x > 0)
        {
            if (x == 6) x--; // skip vertical timing column

            bitIndex = PlaceColumnBits(matrix, bits, size, x, dir, bitIndex);

            x -= 2;
            dir = -dir;
        }

        FillUnsetModules(matrix, size);
    }

    private static List<bool> ExtractBits(byte[] finalCodewords)
    {
        List<bool> bits = [];

        foreach (byte b in finalCodewords)
        {
            for (int i = 7; i >= 0; i--) bits.Add(((b >> i) & 1) == 1);
        }

        return bits;
    }

    private static int PlaceColumnBits(bool?[,] matrix, List<bool> bits, int size, int x, int dir, int bitIndex)
    {
        for (int i = 0; i < size; i++)
        {
            int yy = dir == -1 ? size - 1 - i : i;
            for (int xi = 0; xi < 2; xi++)
            {
                int xx = x - xi;

                if (matrix[yy, xx].HasValue)
                    continue; // function or already set

                bool value = false;

                if (bitIndex < bits.Count)
                    value = bits[bitIndex++];

                matrix[yy, xx] = value;
            }
        }

        return bitIndex;
    }

    private static void FillUnsetModules(bool?[,] matrix, int size)
    {
        for (int yy = 0; yy < size; yy++)
        {
            for (int xx = 0; xx < size; xx++)
            {
                if (!matrix[yy, xx].HasValue)
                    matrix[yy, xx] = false;
            }
        }
    }

    private static bool[,] ConvertNullableToBool(bool?[,] template)
    {
        int s = template.GetLength(0);
        bool[,] outm = new bool[s, s];

        for (int y = 0; y < s; y++)
            for (int x = 0; x < s; x++)
                outm[y, x] = template[y, x].GetValueOrDefault(false);

        return outm;
    }

    // Apply mask: we must not overwrite function modules; matrixFixed tells which entries are fixed (non-null)
    private static void ApplyDataMask(bool[,] matrixCopy, bool?[,] template, int mask)
    {
        int size = matrixCopy.GetLength(0);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                if (template[y, x].HasValue) // function module
                {
                    // function modules are already set in matrixCopy
                    continue;
                }
                bool current = matrixCopy[y, x];
                bool maskBit = MaskPatterns.Mask(mask, x, y);
                matrixCopy[y, x] = current ^ maskBit;
            }
    }
}
