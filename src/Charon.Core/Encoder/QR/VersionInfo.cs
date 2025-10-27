namespace Charon.Encoder.QR;

sealed class VersionInfo
{
    public int Version;
    public int TotalDataCodewords;
    // Block group info etc
    public List<BlockInfo> Blocks = [];
    public int EcCodewordsPerBlock;
}
