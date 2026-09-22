namespace chess;

public static class PrecomputedMoveMaps
{
    public static bool Generated = false;
    public static readonly bitboard[] KnightAttacks = new bitboard[64];
    public static readonly bitboard[] KingAttacks = new bitboard[64];
    public static readonly bitboard[] WhitePawnAttacks = new bitboard[64];
    public static readonly bitboard[] BlackPawnAttacks = new bitboard[64];
    private static readonly int[] KightFileOffsets = { 1, 2, 2, 1, -1, -2, -2, -1 };
    private static readonly int[] KightRankOffsets = { 2, 1, -1, -2, -2, -1, 1, 2 };
    private static readonly int[] KingFileOffsets = { -1, 0, 1, -1, 1, -1, 0, 1 };
    private static readonly int[] KingRankOffsets = { 1, 1, 1, 0, 0, -1, -1, -1 };

    public static bool IsInsideBounds(int r, int f) => f >= 0 && f < 8 && r >= 0 && r < 8;
    public static void InitKnightAttacks()
    {
        for (int i = 0; i < 64; i++)
        {
            int rank = i / 8;
            int file = i % 8;


            for (int k = 0; k < 8; k++)
            {
                int f = file + KightFileOffsets[k];
                int r = rank + KightRankOffsets[k];

                if (IsInsideBounds(r, f)) KnightAttacks[i] |= 1UL << (r * 8 + f);
            }

        }
    }
    public static void InitKingAttacks()
    {
        for (int i = 0; i < 64; i++)
        {
            int rank = i / 8;
            int file = i % 8;

            for (int k = 0; k < 8; k++)
            {
                int r = rank + KingRankOffsets[k];
                int f = file + KingFileOffsets[k];

                if (IsInsideBounds(r, f)) KingAttacks[i] |= 1UL << (r * 8 + f);
            }
        }
    }
    public static void InitPawnAttacks(bitboard[] PawnAttacks, int direction)
    {
        for (int i = 0; i < 64; i++)
        {
            int rank = i / 8;
            int file = i % 8;

            int r = rank + direction;
            int f = file + 1;
            if (IsInsideBounds(r, f)) PawnAttacks[i] |= 1UL << (r * 8 + f);

            f = file - 1;
            if (f >= 0 && f < 8) PawnAttacks[i] |= 1UL << (r * 8 + f);

        }
    }

    public static void InitAttacks()
    {
        InitKnightAttacks();
        InitKingAttacks();
        InitPawnAttacks(WhitePawnAttacks, 1);
        InitPawnAttacks(BlackPawnAttacks, -1);
        Generated = true;
    }
}