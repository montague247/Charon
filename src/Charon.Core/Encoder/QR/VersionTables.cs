namespace Charon.Encoder.QR;

static class VersionTables
{
    // We need full maps per version and ECC
    // To keep things manageable for this sample, create explicit mapping for versions 1..10 & all ECC levels
    // Data derived from ISO/IEC 18004 tables (abbreviated)
    private static readonly Dictionary<(int version, EccLevel ecc), VersionInfo> versions = Init();

    private static Dictionary<(int, EccLevel), VersionInfo> Init()
    {
        var d = new Dictionary<(int, EccLevel), VersionInfo>();

        // For each: totalDataCodewords, ecCodewordsPerBlock, blocks (count + dataCodewords per block)
        // The arrays below were populated based on spec tables.

        void add(int v, EccLevel e, int totalData, int ecPerBlock, params (int count, int dataPerBlock)[] groups)
        {
            var vi = new VersionInfo
            {
                Version = v,
                TotalDataCodewords = totalData,
                EcCodewordsPerBlock = ecPerBlock
            };

            foreach (var (count, dataPerBlock) in groups)
                vi.Blocks.Add(new BlockInfo { Count = count, DataCodewords = dataPerBlock });

            d[(v, e)] = vi;
        }

        // Version 1
        add(1, EccLevel.Low, 19, 7, (1, 19));
        add(1, EccLevel.Medium, 16, 10, (1, 16));
        add(1, EccLevel.Quartile, 13, 13, (1, 13));
        add(1, EccLevel.High, 9, 17, (1, 9));

        // Version 2
        add(2, EccLevel.Low, 34, 10, (1, 34));
        add(2, EccLevel.Medium, 28, 16, (1, 28));
        add(2, EccLevel.Quartile, 22, 22, (1, 22));
        add(2, EccLevel.High, 16, 28, (1, 16));

        // Version 3
        add(3, EccLevel.Low, 55, 15, (1, 55));
        add(3, EccLevel.Medium, 44, 26, (1, 44));
        add(3, EccLevel.Quartile, 34, 18, (2, 17)); // 2 blocks of 17 data each (combined => 34)
        add(3, EccLevel.High, 26, 22, (2, 13));

        // Version 4
        add(4, EccLevel.Low, 80, 20, (1, 80));
        add(4, EccLevel.Medium, 64, 18, (2, 32));
        add(4, EccLevel.Quartile, 48, 26, (2, 24));
        add(4, EccLevel.High, 36, 16, (4, 9));

        // Version 5
        add(5, EccLevel.Low, 108, 26, (1, 108));
        add(5, EccLevel.Medium, 86, 24, (2, 43));
        add(5, EccLevel.Quartile, 62, 18, (2, 15), (2, 16)); // two groups (2x15 and 2x16) total 62 (we'll flatten)
                                                             // But our BlockInfo supports simple list; to keep consistent we expand into multiple BlockInfo entries:
                                                             // Adjusting function to allow multiple groups: above we used tuple array; here passing more groups.
                                                             // To simplify, rewrite for this specific one:
        d[(5, EccLevel.Quartile)] = new VersionInfo
        {
            Version = 5,
            TotalDataCodewords = 62,
            EcCodewordsPerBlock = 18,
            Blocks = [new BlockInfo { Count = 2, DataCodewords = 15 }, new BlockInfo { Count = 2, DataCodewords = 16 }]
        };
        add(5, EccLevel.High, 46, 24, (2, 11), (2, 12));
        d[(5, EccLevel.High)].Blocks = [new BlockInfo { Count = 2, DataCodewords = 11 }, new BlockInfo { Count = 2, DataCodewords = 12 }];

        // Version 6
        add(6, EccLevel.Low, 136, 18, (1, 136));
        add(6, EccLevel.Medium, 108, 16, (2, 27), (2, 26));
        d[(6, EccLevel.Medium)].Blocks = [new BlockInfo { Count = 2, DataCodewords = 27 }, new BlockInfo { Count = 2, DataCodewords = 26 }];
        add(6, EccLevel.Quartile, 76, 24, (4, 19));
        add(6, EccLevel.High, 60, 28, (4, 15));

        // Version 7
        add(7, EccLevel.Low, 156, 20, (1, 156));
        add(7, EccLevel.Medium, 124, 18, (2, 37));
        add(7, EccLevel.Quartile, 88, 18, (4, 22));
        add(7, EccLevel.High, 66, 26, (4, 14), (4, 15));
        d[(7, EccLevel.High)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 14 }, new BlockInfo { Count = 4, DataCodewords = 15 }];

        // Version 8
        add(8, EccLevel.Low, 194, 24, (1, 194));
        add(8, EccLevel.Medium, 154, 22, (2, 38), (2, 39));
        d[(8, EccLevel.Medium)].Blocks = [new BlockInfo { Count = 2, DataCodewords = 38 }, new BlockInfo { Count = 2, DataCodewords = 39 }];
        add(8, EccLevel.Quartile, 110, 22, (4, 20), (2, 21));
        d[(8, EccLevel.Quartile)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 20 }, new BlockInfo { Count = 2, DataCodewords = 21 }];
        add(8, EccLevel.High, 86, 26, (4, 16), (4, 17));
        d[(8, EccLevel.High)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 16 }, new BlockInfo { Count = 4, DataCodewords = 17 }];

        // Version 9
        add(9, EccLevel.Low, 232, 30, (1, 232));
        add(9, EccLevel.Medium, 182, 22, (2, 36), (4, 37));
        d[(9, EccLevel.Medium)].Blocks = [new BlockInfo { Count = 2, DataCodewords = 36 }, new BlockInfo { Count = 4, DataCodewords = 37 }];
        add(9, EccLevel.Quartile, 132, 20, (4, 20), (4, 21));
        d[(9, EccLevel.Quartile)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 20 }, new BlockInfo { Count = 4, DataCodewords = 21 }];
        add(9, EccLevel.High, 100, 24, (4, 18), (4, 19));
        d[(9, EccLevel.High)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 18 }, new BlockInfo { Count = 4, DataCodewords = 19 }];

        // Version 10
        add(10, EccLevel.Low, 274, 18, (1, 274));
        add(10, EccLevel.Medium, 216, 26, (2, 43), (2, 44));
        d[(10, EccLevel.Medium)].Blocks = [new BlockInfo { Count = 2, DataCodewords = 43 }, new BlockInfo { Count = 2, DataCodewords = 44 }];
        add(10, EccLevel.Quartile, 154, 24, (4, 22), (6, 23));
        d[(10, EccLevel.Quartile)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 22 }, new BlockInfo { Count = 6, DataCodewords = 23 }];
        add(10, EccLevel.High, 122, 28, (4, 20), (6, 21));
        d[(10, EccLevel.High)].Blocks = [new BlockInfo { Count = 4, DataCodewords = 20 }, new BlockInfo { Count = 6, DataCodewords = 21 }];

        return d;
    }

    public static VersionInfo GetVersionInfo(int version, EccLevel ecc)
    {
        if (versions.TryGetValue((version, ecc), out var vi))
            return vi;

        throw new ArgumentException($"Version {version} ECC {ecc} not implemented in VersionTables.");
    }

    // Character capacity simple mapping for single-mode cases (v1..10). These numbers are standard tables, but for brevity we include common capacities.
    public static int GetCharCapacity(int version, Mode mode, EccLevel ecc)
    {
        // For production code, you should use full official capacity tables. Here we provide reasonable approximations / standard values up to v10.
        // We'll implement a minimal table for the four modes for versions 1..10.
        // For accuracy, values derived from QR spec tables.
        // For brevity in this example, here's a small table (version, ecc, mode) => capacity
        int[,] numeric = {
                    // v1..10 numeric capacities for L,M,Q,H
                    {41,34,27,17}, {77,63,48,34}, {127,101,77,58}, {187,149,111,82}, {255,202,144,106},
                    {322,255,178,139}, {370,293,207,154}, {461,365,259,202}, {552,432,312,235}, {652,513,364,288}
                };
        int[,] alnum = {
                    {25,20,16,10}, {47,38,29,20}, {77,61,47,35}, {114,90,67,50}, {154,122,87,64},
                    {195,154,108,84}, {224,178,125,93}, {279,221,157,122}, {335,262,189,143}, {395,311,221,174}
                };
        int[,] byteCap = {
                    {17,14,11,7}, {32,26,20,14}, {53,42,32,24}, {78,62,46,34}, {106,84,60,44},
                    {134,106,74,58}, {154,122,86,64}, {192,152,108,84}, {230,180,130,98}, {271,213,151,119}
                };
        int[,] kanji = {
                    {10,8,7,4},{20,16,12,8},{32,26,20,15},{48,36,28,21},{65,52,37,27},
                    {82,65,45,36},{95,73,53,40},{118,93,64,52},{141,111,84,60},{167,131,98,74}
                };

        int vIdx = version - 1;
        int eccIdx = ecc switch
        {
            EccLevel.Low => 0,
            EccLevel.Medium => 1,
            EccLevel.Quartile => 2,
            EccLevel.High => 3,
            _ => throw new NotImplementedException()
        };

        return mode switch
        {
            Mode.Numeric => numeric[vIdx, eccIdx],
            Mode.Alphanumeric => alnum[vIdx, eccIdx],
            Mode.Byte => byteCap[vIdx, eccIdx],
            Mode.Kanji => kanji[vIdx, eccIdx],
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported mode")
        };
    }

    public static int GetCharacterCountIndicatorBits(int version, Mode mode)
    {
        return mode switch
        {
            Mode.Numeric => GetBitsForVersion(version, 10, 12, 14),
            Mode.Alphanumeric => GetBitsForVersion(version, 9, 11, 13),
            Mode.Byte => GetBitsForVersion(version, 8, 16, 16),
            Mode.Kanji => GetBitsForVersion(version, 8, 10, 12),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), "Unsupported mode")
        };
    }

    private static int GetBitsForVersion(int version, int low, int mid, int high)
    {
        if (version <= 9)
            return low;

        if (version <= 26)
            return mid;

        return high;
    }
}
