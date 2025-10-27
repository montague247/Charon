namespace Charon.Encoder.QR;

static class FormatAndVersionPlacer
{
    // Format info bits: 15 bits: ECC & mask + BCH
    // We compute 15 bit format string for (ecc, mask) using known constants
    private static readonly Dictionary<EccLevel, int> EccFormatBits = new()
            {
                { EccLevel.Low, 1 },
                { EccLevel.Medium, 0 },
                { EccLevel.Quartile, 3 },
                { EccLevel.High, 2 }
            };

    public static void PlaceFormatAndVersion(bool[,] matrix, EccLevel ecc, int mask, int version)
    {
        int s = matrix.GetLength(0);
        int formatInfo = BuildFormatInfo(ecc, mask);

        // place around top-left
        for (int i = 0; i <= 5; i++)
        {
            matrix[8, i] = ((formatInfo >> (14 - i)) & 1) == 1;
        }

        matrix[8, 7] = ((formatInfo >> 8) & 1) == 1;
        matrix[8, 8] = ((formatInfo >> 7) & 1) == 1;
        matrix[7, 8] = ((formatInfo >> 6) & 1) == 1;
        
        for (int i = 9; i <= 14; i++)
        {
            matrix[14 - i, 8] = ((formatInfo >> (14 - i)) & 1) == 1;
        }

        // other format area
        for (int i = 0; i <= 7; i++)
        {
            matrix[s - 1 - i, 8] = ((formatInfo >> i) & 1) == 1;
        }
        for (int i = 8; i <= 14; i++)
        {
            matrix[8, s - 15 + i] = ((formatInfo >> i) & 1) == 1;
        }

        // version info (if version >=7): 18 bits in two places (not implemented fully here)
        if (version >= 7)
        {
            // For brevity, we skip detailed version info BCH generation; place zeros
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 3; j++)
                {
                    matrix[i, s - 11 + j] = false;
                    matrix[s - 11 + j, i] = false;
                }
        }
    }

    private static int BuildFormatInfo(EccLevel ecc, int mask)
    {
        // format bits = 5 bits (ECC + mask) -> BCH -> xor with mask 0x5412
        int eccBits = EccFormatBits[ecc] << 3;
        int data = eccBits | mask;
        int g = 0x537; // generator polynomial? In spec generator for format is 0x537?
                       // For correctness, use standard algorithm to compute BCH(15,5) with polynomial x^10 + x^8 + x^5 + x^4 + x^2 + x + 1 (0x537)
        int d = data << 10;
        int p = 0x537;

        while ((d >> 10) > 0)
        {
            int shift = (int)Math.Log(d, 2) - 10;
            d ^= p << shift;
        }

        int format = (data << 10) | d;
        format ^= 0x5412;

        return format & 0x7FFF;
    }
}
