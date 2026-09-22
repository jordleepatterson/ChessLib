namespace chess;

public class MoveHandler
{
    private Board board;
    private BitboardSet bitboards;
    public MoveHandler(Board board)
    {
        this.board = board;
        bitboards = board.bitboards;

    }
    public void MakeMove(Move move)
    {
        MoveFlags MoveFlag = GenerateMoveFlag(move);

        switch (MoveFlag)
        {
            case MoveFlags.Normal: MoveNormal(move); break;
            case MoveFlags.Capture: Capture(move); break;
            case MoveFlags.Castle: Castle(move); break;
        }
    }
    private MoveFlags GenerateMoveFlag(Move move)
    {
        MoveMap moveMap = new MoveMap(board, move.From);
        moveMap.GenerateLegalMoves(board.moveValidator);

        bitboard fromMask = 1UL << move.From;
        bitboard toMask = 1Ul << move.To;

        if ((moveMap.castling & toMask) != 0) return MoveFlags.Castle;
        if ((moveMap.captures & toMask) != 0) return MoveFlags.Capture;
        return MoveFlags.Normal;

    }
    public void Capture(Move move)
    {
        int movingPiece = board.GetBitboardIndex(move.From);
        int capturedPiece = board.GetBitboardIndex(move.To);

        if (movingPiece == -1 || capturedPiece == -1) return;

        bitboards[movingPiece] &= ~(1UL << move.From);
        bitboards[capturedPiece] &= ~(1UL << move.To);
        bitboards[movingPiece] |= 1UL << move.To;
    }
    public void MoveNormal(Move move)
    {
        int movingPiece = board.GetBitboardIndex(move.From);

        if (movingPiece == -1) return;

        bitboards[movingPiece] &= ~(1UL << move.From);
        bitboards[movingPiece] |= 1UL << move.To;
    }

    public void MoveAndCapture(Move move)
    {
        int movingPiece = board.GetBitboardIndex(move.From);
        int capturedPiece = board.GetBitboardIndex(move.To);

        bitboard fromMask = 1UL << move.From;
        bitboard toMask = 1UL << move.To;

        if (movingPiece == -1)
            return;

        if (capturedPiece != -1)
            bitboards[capturedPiece] &= ~toMask;

        bitboards[movingPiece] &= ~fromMask;
        bitboards[movingPiece] |= toMask;
    }
    public void Castle(Move move)
    {
        if (move.To == (int)g1 || move.To == (int)g8) CastleKingSide();
        else
            if (move.To == (int)c1 || move.To == (int)c8) CastleQueenSide();
    }
    public void CastleKingSide()
    {

        if (board.Turn == white)
        {
            bitboards.wKing &= ~(1UL << (int)e1);
            bitboards.wRooks &= ~(1UL << (int)h1);

            bitboards.wKing |= 1UL << (int)g1;
            bitboards.wRooks |= 1UL << (int)f1;
        }
        else
        {
            bitboards.bKing &= ~(1UL << (int)e8);
            bitboards.bRooks &= ~(1UL << (int)h8);

            bitboards.bKing |= 1UL << (int)g8;
            bitboards.bRooks |= 1UL << (int)f8;
        }
    }
    public void CastleQueenSide()
    {
        if (board.Turn == white)
        {
            bitboards.wKing &= ~(1UL << (int)e1);
            bitboards.wRooks &= ~(1UL << (int)a1);

            bitboards.wKing |= 1UL << (int)c1;
            bitboards.wRooks |= 1UL << (int)d1;
        }
        else
        {
            bitboards.bKing &= ~(1UL << (int)e8);
            bitboards.bRooks &= ~(1UL << (int)a8);

            bitboards.bKing |= 1UL << (int)c8;
            bitboards.bRooks |= 1UL << (int)d8;
        }
    }
}