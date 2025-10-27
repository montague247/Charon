namespace Charon.Encoder.QR;

sealed class BitBuffer
{
    private readonly List<bool> _bits = [];

    public int Count => _bits.Count;

    public IEnumerable<bool> Bits => _bits;

    public void Put(int value, int bitCount)
    {
        for (int i = bitCount - 1; i >= 0; i--)
            _bits.Add(((value >> i) & 1) == 1);
    }

    public void PutBits(IEnumerable<bool> moreBits) => _bits.AddRange(moreBits);

    public byte[] ToBytes()
    {
        int byteCount = (_bits.Count + 7) / 8;
        byte[] outb = new byte[byteCount];

        for (int i = 0; i < _bits.Count; i++)
        {
            if (_bits[i])
            {
                outb[i / 8] |= (byte)(1 << (7 - (i % 8)));
            }
        }

        return outb;
    }
}
