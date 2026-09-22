namespace chess;

public static class ChessLog
{
    public static void PrintBitBoard(bitboard board)
    {
        Console.WriteLine();
        for (int rank = 0; rank < 8; rank++)
        {
            Console.Write(7 - rank + 1 + "  ");
            for (int file = 0; file < 8; file++)
            {
                int bit = rank * 8 + file;
                bool occupied = ((board >> bit) & 1) != 0;

                Console.Write(occupied ? "# " : ". ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("\n   A B C D E F G H\n");
    }
    public static void PrintBoard(Board board)
    {
        Console.WriteLine();
        for (int rank = 0; rank < 8; rank++)
        {
            Console.Write(7 - rank + 1 + "  ");
            for (int file = 0; file < 8; file++)
            {
                for (int i = 0; i < 12; i++)
                {
                    int pos = rank * 8 + file;
                    if ((board.bitboards[i] & (1UL << pos)) != 0)
                    {
                        string Piece = i switch
                        {
                            (int)whitePawns => "P ",
                            (int)whiteKnights => "N ",
                            (int)whiteBishops => "B ",
                            (int)whiteRooks => "R ",
                            (int)whiteQueens => "Q ",
                            (int)whiteKing => "K ",
                            (int)blackPawns => "p ",
                            (int)blackKnights => "n ",
                            (int)blackBishops => "b ",
                            (int)blackRooks => "r ",
                            (int)blackQueens => "q ",
                            (int)blackKing => "k ",
                            _ => ". "
                        };
                        Console.Write(Piece);
                        break;
                    }
                    else if (i == 11) Console.Write(". ");
                }
            }
            if (rank == 1) Console.Write("          CasltingRights: " + board.castlingRights);
            if (rank == 0) Console.Write("                    Turn: " + board.Turn);
            Console.WriteLine();
        }
        Console.WriteLine("\n   A B C D E F G H \n");
    }
    public static void DisplayAllMoves(Board board)
    {
        for (int i = 0; i < 64; i++)
        {
            if ((board.bitboards.Occupied & (1UL << i)) != 0)
            {
                MoveMap moveMap = new MoveMap(board, i);
                moveMap.GenerateMoves();
                Console.WriteLine(board.GetPieceColor(i) + "_" + board.GetPieceType(i) + " :");
                PrintBitBoard(moveMap.moves);
            }
        }
    }
    public static void DisplayWhiteAttacks(Board board)
    {
        bitboard moves = 0;
        for (int i = 0; i < 64; i++)
        {
            if ((board.bitboards.wPieces & (1UL << i)) != 0)
            {
                MoveMap moveMap = new MoveMap(board, i);
                moveMap.GenerateMoves();
                moves |= moveMap.moves;
            }
        }
        PrintBitBoard(moves);
    }
    public static void DisplayBlackAttacks(Board board)
    {
        bitboard moves = 0;
        for (int i = 0; i < 64; i++)
        {
            if ((board.bitboards.bPieces & (1UL << i)) != 0)
            {
                MoveMap moveMap = new MoveMap(board, i);
                moveMap.GenerateMoves();
                moves |= moveMap.moves;
            }
        }
        PrintBitBoard(moves);
    }
}
