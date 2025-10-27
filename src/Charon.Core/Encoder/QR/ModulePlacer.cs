namespace Charon.Encoder.QR;

static class ModulePlacer
{
    public static void PlaceFunctionPatterns(bool?[,] matrix, int version)
    {
        int size = matrix.GetLength(0);
        PlaceFinder(matrix, 0, 0);
        PlaceFinder(matrix, size - 7, 0);
        PlaceFinder(matrix, 0, size - 7);
        PlaceSeparators(matrix);
        PlaceTiming(matrix);
        PlaceAlignment(matrix, version);

        // dark module
        matrix[8, size - 8] = true;

        // reserve format info areas and version info areas as non-null (will be overwritten later by format/version bits)
        ReserveFormatAreas(matrix);

        if (version >= 7)
            ReserveVersionAreas(matrix);
    }

    private static void PlaceFinder(bool?[,] m, int x, int y)
    {
        for (int dy = 0; dy < 7; dy++)
            for (int dx = 0; dx < 7; dx++)
            {
                bool val = dx == 0 || dx == 6 || dy == 0 || dy == 6 || (dx >= 2 && dx <= 4 && dy >= 2 && dy <= 4);
                m[y + dy, x + dx] = val;
            }
    }

    private static void PlaceSeparators(bool?[,] m)
    {
        int s = m.GetLength(0);
        // around top-left
        for (int i = 0; i < 8; i++) { m[i, 7] = false; m[7, i] = false; }
        // top-right
        for (int i = 0; i < 8; i++) { m[i, s - 8] = false; m[7, s - 1 - i] = false; }
        // bottom-left
        for (int i = 0; i < 8; i++) { m[s - 8, i] = false; m[s - 1 - i, 7] = false; }
    }

    private static void PlaceTiming(bool?[,] m)
    {
        int s = m.GetLength(0);
        for (int i = 8; i < s - 8; i++)
        {
            bool v = (i % 2 == 0);
            m[6, i] = v;
            m[i, 6] = v;
        }
    }

    private static readonly int[]?[] AlignmentLocationsTable =
    [
        null,
        [], // v1 none
        [6,18], // v2
        [6,22],
        [6,26],
        [6,30],
        [6,34],
        [6,22,38],
        [6,24,42],
        [6,26,46],
        [6,28,50]
        // extend for higher versions...
    ];

    private static void PlaceAlignment(bool?[,] m, int version)
    {
        if (version == 1)
            return;

        int[]? locs = AlignmentLocationsTable.Length > version ? AlignmentLocationsTable[version] : null;

        if (locs == null || locs.Length == 0)
            return;

        foreach (int r in locs)
            foreach (int c in locs)
            {
                // skip if overlapping with finder patterns
                if ((r == 6 && (c == 6 || c == m.GetLength(0) - 7 || c == 0)) || (c == 6 && (r == 6 || r == m.GetLength(0) - 7 || r == 0))) continue;
                // place 5x5 pattern centered at (r,c)
                for (int dy = -2; dy <= 2; dy++)
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        int y = r + dy, x = c + dx;
                        if (y < 0 || x < 0 || y >= m.GetLength(0) || x >= m.GetLength(0)) continue;
                        bool val = Math.Max(Math.Abs(dx), Math.Abs(dy)) != 1; // rough approximation: outer 5x5 black/white pattern with center black and inner white ring
                                                                              // Spec has exact pattern; for many encoders this simplified placement suffices for patterns
                        m[y, x] = val;
                    }
            }
    }

    private static void ReserveFormatAreas(bool?[,] m)
    {
        int s = m.GetLength(0);
        // format info adjacent to finders
        for (int i = 0; i <= 8; i++)
        {
            if (i != 6) m[8, i] = false; // will be overwritten by format
            if (i != 6) m[i, 8] = false;
        }
        for (int i = 0; i < 8; i++)
        {
            m[s - 1 - i, 8] = false;
            m[8, s - 1 - i] = false;
        }
    }

    private static void ReserveVersionAreas(bool?[,] m)
    {
        int s = m.GetLength(0);
        // top-right
        for (int r = 0; r < 6; r++)
            for (int c = s - 11; c < s - 8; c++) m[r, c] = false;
        // bottom-left
        for (int r = s - 11; r < s - 8; r++)
            for (int c = 0; c < 6; c++) m[r, c] = false;
    }
}
