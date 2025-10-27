namespace Charon.Encoder.QR;

static class MaskPatterns
{
    public static bool Mask(int mask, int x, int y)
    {
        return mask switch
        {
            0 => (x + y) % 2 == 0,
            1 => y % 2 == 0,
            2 => x % 3 == 0,
            3 => (x + y) % 3 == 0,
            4 => ((y / 2) + (x / 3)) % 2 == 0,
            5 => ((x * y) % 2) + ((x * y) % 3) == 0,
            6 => (((x * y) % 2) + ((x * y) % 3)) % 2 == 0,
            7 => (((x + y) % 2) + ((x * y) % 3)) % 2 == 0,
            _ => false
        };
    }
}
