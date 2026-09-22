namespace chess;

public class MoveValidator
{
    public readonly Board board;
    public readonly Move move;
    private readonly bitboard Occupied;

    public MoveValidator(Board board, Move move)
    {
        this.board = board;
        this.move = move;
        Occupied = board.bitboards.Occupied;
    }
    public bool InCheck(Move move)
    {
        Board TempBoard = board.Clone();

        TempBoard.moveHandler.MoveNormal(move);

        for (int i = 0; i < 64; i++)
        {
            for (int k = 0; k < 6; k++)
            {
                if ((TempBoard.bitboards[k + (int)TempBoard.Turn] & (1UL << i)) != 0)
                {
                    MoveMap moveMap = new MoveMap(TempBoard, i);
                    moveMap.GenerateAttacks();

                    const int king_offset = 5;
                    bitboard KingMask = TempBoard.bitboards[(int)TempBoard.opTurn() + king_offset];

                    if ((KingMask & moveMap.attacks) != 0)
                        return true;
                    else
                        break;
                }
            }
        }
        return false;
    }
    public bool IsLegal(Move move)
    {
        bitboard FromMask = 1UL << move.From;
        bitboard ToMask = 1UL << move.To;
        bitboard whitePieces = board.bitboards.wPieces;
        bitboard blackPieces = board.bitboards.bPieces;

        bool IsSamePieceColor =
            (whitePieces & FromMask) != 0 && (whitePieces & ToMask) != 0 ||
            (blackPieces & FromMask) != 0 && (blackPieces & ToMask) != 0;

        return !IsSamePieceColor && !InCheck(move) &&
                (Occupied & FromMask) == 0 &&
                move.From != move.To;
    }
    public bool IsValid(Move move)
    {
        if (board.GetPieceColor(move.From) != board.Turn) return false;

        MoveMap moveMap = new MoveMap(board, move.From);
        moveMap.GenerateMoves();

        bool IsInMoveMap = (moveMap.castling & 1UL << move.To) != 0 || (moveMap.moves & 1UL << move.To) != 0;

        if (IsInMoveMap && IsLegal(move))
            return true;
        else
            return false;
    }
    public MoveValidator(Board board)
    {
        this.board = board;
        Occupied = board.bitboards.Occupied;
    }
    public bool IsValid() => IsValid(move);

}