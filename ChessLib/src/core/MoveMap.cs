using System.ComponentModel;
namespace chess;

public class MoveMap
{
    public bitboard moves { get; private set; }
    public bitboard legalMoves { get; private set; }
    public bitboard legalCaptures { get; private set; }
    public bitboard castling { get; private set; }
    public bitboard captures { get; private set; }
    public bitboard attacks { get; private set; }


    public readonly int piecePosition;
    private readonly bitboard Occupied;
    private Board board;
    private static bool IsInsideBounds(int r, int f) => f >= 0 && f < 8 && r >= 0 && r < 8;
    public MoveMap(Board board, int piecePosition)
    {
        this.board = board;
        this.piecePosition = piecePosition;
        Occupied = board.bitboards.Occupied;
        InitMaps();
    }
    public void GenerateLegalMoves(int position, MoveValidator moveValidator)
    {
        if (moveValidator.board != board)
            throw new WarningException("moveValidator.board != MoveGenerator.board");

        GenerateMoves(position);
        MakeLegal(moveValidator, position);
    }
    private void MakeLegal(MoveValidator moveValidator, int position)
    {
        for (int i = 0; i < 64; i++)
        {
            if ((moves & (1UL << i)) != 0)
            {
                Move tempMove = new Move(position, i);

                if (moveValidator.IsLegal(tempMove))
                    legalMoves |= 1UL << i;
            }
            if ((captures & (1UL << i)) != 0)
            {
                Move tempMove = new Move(position, i);

                if (moveValidator.IsLegal(tempMove))
                    legalCaptures |= 1UL << i;
            }
        }
    }
    public void GenerateAttacks(int position)
    {
        InitMaps();

        PieceType piece = board.GetPieceType(position);

        switch (piece)
        {
            case pawn: GeneratePawnMoves(position); break;
            case knight: GenerateKnightAttacks(position); break;
            case bishop: GenerateBishopAttacks(position); break;
            case rook: GenerateRookAttacks(position); break;
            case queen: GenerateQueenAttacks(position); break;
            case king: GenerateKingAttacks(position); break;

        }
        ;
    }
    public void GenerateMoves(int position)
    {
        InitMaps();

        PieceType piece = board.GetPieceType(position);

        switch (piece)
        {
            case pawn: GeneratePawnAttacks(position); break;
            case knight: GenerateKnightAttacks(position); break;
            case bishop: GenerateBishopAttacks(position); break;
            case rook: GenerateRookAttacks(position); break;
            case queen: GenerateQueenAttacks(position); break;
            case king: GenerateKingAttacks(position); GenerateCastling(position); break;

        }
        ;
    }
    public void GeneratePawnMoves(int position)
    {
        int rank = position / 8;
        int file = position % 8;

        int dir;
        int StartRank;

        if (board.GetPieceColor(position) == white)
        {
            dir = -1;
            StartRank = 6;
        }
        else
        {
            dir = 1;
            StartRank = 1;
        }

        int square = (rank + dir) * 8 + file;

        if ((Occupied & (1UL << square)) == 0)
        {
            moves |= 1UL << square;

            if (rank == StartRank)
            {
                square = (rank + (dir * 2)) * 8 + file;

                if ((Occupied & (1UL << square)) == 0)
                {
                    moves |= 1UL << square;
                }
            }
        }
        GeneratePawnAttacks(position);
    }
    public void GeneratePawnAttacks(int position)
    {
        if (board.Turn == white)
            attacks = PrecomputedMoveMaps.WhitePawnAttacks[position];
        else
            attacks = PrecomputedMoveMaps.BlackPawnAttacks[position];
    }
    public void GenerateKingAttacks(int position)
    {
        attacks = PrecomputedMoveMaps.KingAttacks[position];
        moves = attacks;
    }
    public void GenerateCastling(int position)
    {
        if (board.GetPieceColor(position) == white)
        {
            if (board.CanCastle(WhiteKingSide))
                castling |= 1UL << (int)g1;
            if (board.CanCastle(WhiteQueenSide))
                castling |= 1UL << (int)c1;
        }
        else
        {
            if (board.CanCastle(BlackKingSide))
                castling |= 1UL << (int)g8;
            if (board.CanCastle(BlackQueenSide))
                castling |= 1UL << (int)c8;
        }

    }
    public void GenerateKnightAttacks(int position)
    {
        attacks = PrecomputedMoveMaps.KnightAttacks[position];
        moves = attacks;

        for (int i = 0; i < 64; i++)
            if ((attacks & (1UL << i)) != 0)
                captures |= 1UL << i;

    }
    public void GenerateBishopAttacks(int position)
    {
        int[][] directions = {
            new int[] { 1, 1 }, new int[] { -1, 1 }, new int[] { 1, -1 }, new int[] { -1, -1 }
        };
        GenerateRayAttacks(position, directions);
    }
    public void GenerateRookAttacks(int position)
    {
        int[][] directions = {
            new int[] { 1, 0 }, new int[] { -1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 }
        };
        GenerateRayAttacks(position, directions);
    }
    public void GenerateQueenAttacks(int position)
    {
        int[][] directions =
        {
            new int[]{1,1},new int[]{-1,1},new int[]{1,-1},new int[]{-1,-1},
            new int[]{1,0},new int[]{-1,0},new int[]{0,-1},new int[]{0,1}
        };
        GenerateRayAttacks(position, directions);
    }
    public void GenerateRayAttacks(int position, int[][] directions)
    {
        int rank = position / 8;
        int file = position % 8;

        foreach (int[] dir in directions)
        {
            for (int r = rank + dir[0], f = file + dir[1];
                IsInsideBounds(r, f); r += dir[0], f += dir[1])
            {
                int square = r * 8 + f;
                bitboard bit = 1UL << square;

                moves |= bit;
                attacks |= bit;

                if ((Occupied & bit) != 0)
                {
                    captures |= bit;
                    break;
                }
            }
        }
    }
    private void InitMaps()
    {
        moves = 0;
        castling = 0;
        captures = 0;
        attacks = 0;
        legalMoves = 0;
        legalCaptures = 0;
    }
    public void GeneratePawnMoves() => GeneratePawnMoves(piecePosition);
    public void GenerateBishopAttacks() => GenerateBishopAttacks(piecePosition);
    public void GenerateKnightAttacks() => GenerateKnightAttacks(piecePosition);
    public void GenerateRookAttacks() => GenerateRookAttacks(piecePosition);
    public void GenerateQueenAttacks() => GenerateQueenAttacks(piecePosition);
    public void GenerateKingMoves() => GenerateKingAttacks(piecePosition);
    public void GenerateMoves() => GenerateMoves(piecePosition);
    public void GenerateAttacks() => GenerateAttacks(piecePosition);
    public void GenerateLegalMoves(MoveValidator moveValidator) =>
        GenerateLegalMoves(piecePosition, moveValidator);
    public void MakeLegal(MoveValidator moveValidator) => MakeLegal(moveValidator, piecePosition);
}