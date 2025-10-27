namespace Charon.Encoder.QR;

static class ErrorCorrection
{
    // GF(256) tables
    private static readonly byte[] Exp = new byte[512];
    private static readonly byte[] Log = new byte[256];

    static ErrorCorrection()
    {
        // initialize tables with primitive 0x11d
        byte x = 1;
        
        for (int i = 0; i < 255; i++)
        {
            Exp[i] = x;
            Log[x] = (byte)i;
            x = (byte)(x << 1);
            if ((x & 0x100) != 0) x = (byte)(x ^ 0x11d);
        }

        for (int i = 255; i < 512; i++) Exp[i] = Exp[i - 255];
    }

    private static byte GfMul(byte a, byte b)
    {
        if (a == 0 || b == 0) return 0;
        int res = Log[a] + Log[b];
        return Exp[res];
    }

    private static byte GfPow(byte a, int power)
    {
        if (power == 0) return 1;
        if (a == 0) return 0;
        int res = (Log[a] * power) % 255;
        return Exp[res];
    }

    private static byte[] GenerateGeneratorPoly(int degree)
    {
        // start with [1]
        List<byte> gen = [1];

        for (int i = 0; i < degree; i++)
        {
            List<byte> next = new(gen.Count + 1);

            for (int j = 0; j < gen.Count; j++)
                next.Add(gen[j]); // copy
                                  // multiply gen by (x - alpha^i)

            for (int j = 0; j < gen.Count; j++)
            {
                byte a = gen[j];
                byte mul = GfMul(a, Exp[i]);

                // shift and xor
                if (j + 1 >= next.Count)
                    next.Add(mul);
                else
                    next[j + 1] ^= mul;
            }

            gen = next;
        }

        return [.. gen];
    }

    /// <summary>
    /// Given data bytes and versionInfo, split into blocks and compute ECC for each block.
    /// Returns list of block byte arrays (dataBytes[], eccBytes[])
    /// </summary>
    public static List<(byte[] Data, byte[] Ecc)> InterleaveAndGenerate(byte[] dataCodewords, VersionInfo vi)
    {
        // Build explicit blocks as per vi.Blocks
        List<byte[]> dataBlocks = [];
        int offset = 0;

        foreach (var b in vi.Blocks)
        {
            for (int i = 0; i < b.Count; i++)
            {
                byte[] d = new byte[b.DataCodewords];
                Array.Copy(dataCodewords, offset, d, 0, b.DataCodewords);
                dataBlocks.Add(d);
                offset += b.DataCodewords;
            }
        }

        List<(byte[] Data, byte[] Ecc)> result = [];

        foreach (var db in dataBlocks)
        {
            byte[] ecc = ReedSolomonCompute(db, vi.EcCodewordsPerBlock);
            result.Add((db, ecc));
        }

        return result;
    }

    public static byte[] InterleaveBlocks(List<(byte[] Data, byte[] Ecc)> blocks)
    {
        // interleave data codewords
        int maxDataLen = blocks.Max(b => b.Data.Length);
        List<byte> outb = [];

        for (int i = 0; i < maxDataLen; i++)
        {
            foreach (var (Data, Ecc) in blocks)
            {
                if (i < Data.Length)
                    outb.Add(Data[i]);
            }
        }

        // interleave ecc
        int eccLen = blocks[0].Ecc.Length;

        for (int i = 0; i < eccLen; i++)
        {
            foreach (var (Data, Ecc) in blocks)
            {
                outb.Add(Ecc[i]);
            }
        }

        return [.. outb];
    }

    private static byte[] ReedSolomonCompute(byte[] data, int ecCodewords)
    {
        var gen = GenerateGeneratorPoly(ecCodewords);
        byte[] msg = new byte[data.Length + ecCodewords];
        Array.Copy(data, 0, msg, 0, data.Length);

        for (int i = 0; i < data.Length; i++)
        {
            byte factor = msg[i];

            if (factor != 0)
            {
                for (int j = 0; j < gen.Length; j++)
                {
                    msg[i + 1 + j] ^= GfMul(gen[j], factor);
                }
            }
        }

        byte[] ecc = new byte[ecCodewords];
        Array.Copy(msg, data.Length, ecc, 0, ecCodewords);

        return ecc;
    }
}
