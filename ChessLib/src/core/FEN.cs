using System;
using System.Text;
namespace chess;

public static class FEN
{
    public const string StartPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w";
    public static string ToFen(Board board)
    {
        StringBuilder stringBuilder = new StringBuilder();

        char Piece;
        int Empty = 0;

        for (int i = 0; i < 64; i++)
        {
            for (int k = 0; k < 12; k++)
            {
                if ((board.bitboards[k] & (1UL << i)) != 0)
                {
                    if (Empty > 0)
                    {
                        stringBuilder.Append(Empty);
                        Empty = 0;
                    }
                    Piece = k switch
                    {
                        (int)whitePawns => 'P',
                        (int)whiteKnights => 'N',
                        (int)whiteBishops => 'B',
                        (int)whiteRooks => 'R',
                        (int)whiteQueens => 'Q',
                        (int)whiteKing => 'K',
                        (int)blackPawns => 'p',
                        (int)blackKnights => 'n',
                        (int)blackBishops => 'b',
                        (int)blackRooks => 'r',
                        (int)blackQueens => 'q',
                        (int)blackKing => 'k',
                        _ => ' '
                    };

                    stringBuilder.Append(Piece);
                    break;
                }
                else if (k == 11)
                {
                    Empty++;
                }
            }

            if (i % 8 == 7)
            {
                if (Empty > 0)
                {
                    stringBuilder.Append(Empty);
                    Empty = 0;
                }
                if (i / 8 < 7)
                    stringBuilder.Append("/");
            }
        }
        stringBuilder.Append(" ");

        if (board.Turn == white)
            stringBuilder.Append("w");
        else
            stringBuilder.Append("b");

        return stringBuilder.ToString();
    }
    public static Board LoadFromFen(string FenString)
    {
        Board board = new Board();
        string[] FenStrPartitions = FenString.Split(" ");

        char[] CharBoard = ParseBoard(FenStrPartitions[0]);

        for (int i = 0; i < 64; i++)
        {
            char piece = CharBoard[i];
            PieceType pieceType;
            PieceColor pieceColor;

            pieceType = char.ToLower(piece) switch
            {
                'p' => pawn,
                'b' => bishop,
                'n' => knight,
                'r' => rook,
                'q' => queen,
                'k' => king,
                _ => empty
            };

            if (char.IsUpper(piece))
                pieceColor = white;
            else
                pieceColor = black;

            if (pieceType != empty)
                board.CreatePiece(pieceType, pieceColor, i);

        }

        return board;

    }
    public static char[] ParseBoard(string boardpart)
    {
        char[] Board = new char[64];

        string[] ranks = boardpart.Split("/");

        if (ranks.Length != 8)
            throw new ArgumentException("Invalid board layout in FEN");

        for (int rank = 0; rank < ranks.Length; rank++)
        {
            int file = 0;
            foreach (char piece in ranks[rank])
            {
                if (char.IsDigit(piece))
                {
                    int emptySquares = piece - '0';

                    for (int i = 0; i < emptySquares; i++)
                        Board[rank * 8 + file++] = '-';
                }
                else
                    Board[rank * 8 + file++] = piece;


            }
            if (file != 8)
                throw new ArgumentException("Invalid row length in FEN.");
        }
        return Board;
    }
}