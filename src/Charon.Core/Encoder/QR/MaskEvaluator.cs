namespace Charon.Encoder.QR;

/// <summary>
/// Helper class for applying and scoring the 8 QR code mask patterns (ISO/IEC 18004).
/// Expects a square bool[,] matrix: true = dark module, false = light module.
/// </summary>
static class MaskEvaluator
{
    /// <summary>
    /// Applies a given mask pattern (0–7) to the matrix and returns a new masked matrix.
    /// The original matrix remains unchanged.
    /// </summary>
    public static bool[,] ApplyMask(bool[,] matrix, int maskPattern)
    {
        int n = matrix.GetLength(0);
        if (n != matrix.GetLength(1)) throw new ArgumentException("Matrix must be square.");

        bool[,] result = new bool[n, n];
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                bool maskBit = GetMaskBit(maskPattern, r, c);
                // XOR: if maskBit is true, invert the module
                result[r, c] = maskBit ? !matrix[r, c] : matrix[r, c];
            }
        }
        return result;
    }

    /// <summary>
    /// Evaluates all 8 mask patterns and returns the one with the lowest penalty score.
    /// </summary>
    /// <returns>
    /// Tuple: (bestMaskIndex, bestMaskedMatrix, penaltyValue)
    /// </returns>
    public static (int bestMask, bool[,] maskedMatrix, int penalty) SelectBestMask(bool[,] baseMatrix)
    {
        int bestMask = 0;
        bool[,] bestMatrix = null;
        int bestPenalty = int.MaxValue;

        for (int mask = 0; mask < 8; mask++)
        {
            bool[,] masked = ApplyMask(baseMatrix, mask);
            int penalty = CalculatePenalty(masked);
            if (penalty < bestPenalty)
            {
                bestPenalty = penalty;
                bestMask = mask;
                bestMatrix = masked;
            }
        }

        return (bestMask, bestMatrix, bestPenalty);
    }

    /// <summary>
    /// Calculates the total penalty score for a QR matrix using all 4 ISO rules.
    /// </summary>
    public static int CalculatePenalty(bool[,] matrix)
    {
        int n = matrix.GetLength(0);
        int total = 0;
        total += PenaltyRule1(matrix);
        total += PenaltyRule2(matrix);
        total += PenaltyRule3(matrix);
        total += PenaltyRule4(matrix);
        return total;
    }

    #region Mask formulas

    /// <summary>
    /// Returns true if the module at (r,c) should be inverted by the mask.
    /// Mask formulas follow the QR standard.
    /// </summary>
    private static bool GetMaskBit(int maskPattern, int r, int c)
    {
        return maskPattern switch
        {
            0 => ((r + c) % 2) == 0,
            1 => (r % 2) == 0,
            2 => (c % 3) == 0,
            3 => ((r + c) % 3) == 0,
            4 => (((r / 2) + (c / 3)) % 2) == 0,
            5 => ((r * c) % 2 + (r * c) % 3) == 0,
            6 => (((r * c) % 2 + (r * c) % 3) % 2) == 0,
            7 => (((r + c) % 2 + (r * c) % 3) % 2) == 0,
            _ => throw new ArgumentOutOfRangeException(nameof(maskPattern)),
        };
    }

    #endregion

    #region Penalty rules

    // Rule 1: Consecutive modules in a row/column with the same color
    private static int PenaltyRule1(bool[,] m)
    {
        int n = m.GetLength(0);
        int penalty = 0;

        // Rows
        for (int r = 0; r < n; r++)
        {
            int runCount = 1;
            bool last = m[r, 0];
            for (int c = 1; c < n; c++)
            {
                if (m[r, c] == last) runCount++;
                else
                {
                    if (runCount >= 5) penalty += 3 + (runCount - 5);
                    runCount = 1;
                    last = m[r, c];
                }
            }
            if (runCount >= 5) penalty += 3 + (runCount - 5);
        }

        // Columns
        for (int c = 0; c < n; c++)
        {
            int runCount = 1;
            bool last = m[0, c];
            for (int r = 1; r < n; r++)
            {
                if (m[r, c] == last) runCount++;
                else
                {
                    if (runCount >= 5) penalty += 3 + (runCount - 5);
                    runCount = 1;
                    last = m[r, c];
                }
            }
            if (runCount >= 5) penalty += 3 + (runCount - 5);
        }

        return penalty;
    }

    // Rule 2: 2x2 blocks of the same color
    private static int PenaltyRule2(bool[,] m)
    {
        int n = m.GetLength(0);
        int penalty = 0;
        for (int r = 0; r < n - 1; r++)
        {
            for (int c = 0; c < n - 1; c++)
            {
                bool v = m[r, c];
                if (m[r, c + 1] == v && m[r + 1, c] == v && m[r + 1, c + 1] == v)
                {
                    penalty += 3;
                }
            }
        }
        return penalty;
    }

    // Rule 3: Finder-like pattern (1:1:3:1:1) in row/column with 4 white modules before or after
    private static int PenaltyRule3(bool[,] m)
    {
        int n = m.GetLength(0);
        int penalty = 0;

        bool[] pattern = new bool[] { true, false, true, true, true, false, true };
        bool[] inverse = new bool[] { false, true, false, false, false, true, false };

        // Rows
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c <= n - 7; c++)
            {
                if (MatchesPatternRow(m, r, c, pattern) || MatchesPatternRow(m, r, c, inverse))
                {
                    bool preWhite = c - 4 >= 0 && AllWhiteRowSegment(m, r, c - 4, 4);
                    bool postWhite = c + 7 + 4 <= n && AllWhiteRowSegment(m, r, c + 7, 4);
                    if (preWhite || postWhite) penalty += 40;
                }
            }
        }

        // Columns
        for (int c = 0; c < n; c++)
        {
            for (int r = 0; r <= n - 7; r++)
            {
                if (MatchesPatternCol(m, r, c, pattern) || MatchesPatternCol(m, r, c, inverse))
                {
                    bool preWhite = r - 4 >= 0 && AllWhiteColSegment(m, r - 4, c, 4);
                    bool postWhite = r + 7 + 4 <= n && AllWhiteColSegment(m, r + 7, c, 4);
                    if (preWhite || postWhite) penalty += 40;
                }
            }
        }

        return penalty;
    }

    private static bool MatchesPatternRow(bool[,] m, int r, int startC, bool[] pattern)
    {
        for (int k = 0; k < 7; k++) if (m[r, startC + k] != pattern[k]) return false;
        return true;
    }
    private static bool MatchesPatternCol(bool[,] m, int startR, int c, bool[] pattern)
    {
        for (int k = 0; k < 7; k++) if (m[startR + k, c] != pattern[k]) return false;
        return true;
    }
    private static bool AllWhiteRowSegment(bool[,] m, int r, int startC, int length)
    {
        for (int i = 0; i < length; i++) if (m[r, startC + i]) return false;
        return true;
    }
    private static bool AllWhiteColSegment(bool[,] m, int startR, int c, int length)
    {
        for (int i = 0; i < length; i++) if (m[startR + i, c]) return false;
        return true;
    }

    // Rule 4: Balance of dark and light modules (aim for 50% dark)
    private static int PenaltyRule4(bool[,] m)
    {
        int n = m.GetLength(0);
        int total = n * n;
        int dark = 0;
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                if (m[r, c]) dark++;

        double percentDark = (dark * 100.0) / total;
        int fivePercentSteps = (int)(Math.Abs(percentDark - 50) / 5.0 + 0.0000001);
        return fivePercentSteps * 10;
    }

    #endregion
}
