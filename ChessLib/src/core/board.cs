namespace chess;

public sealed class Board
{
    public event Action<Move> AfterMove;
    public static readonly string[] CharBoard =
    {
        "a8","b8","c8","d8","e8","f8","g8","h8",
        "a7","b7","c7","d7","e7","f7","g7","h7",
        "a6","b6","c6","d6","e6","f6","g6","h6",
        "a5","b5","c5","d5","e5","f5","g5","h5",
        "a4","b4","c4","d4","e4","f4","g4","h4",
        "a3","b3","c3","d3","e3","f3","g3","h3",
        "a2","b2","c2","d2","e2","f2","g2","h2",
        "a1","b1","c1","d1","e1","f1","g1","h1"
    };
    public CastlingRights castlingRights;
    public BitboardSet bitboards;
    public PieceColor Turn = white;
    public readonly MoveHandler moveHandler;
    public readonly MoveValidator moveValidator;

    const bitboard WhiteKingSideEmpty = (1UL << (int)f1) | (1UL << (int)g1);
    const bitboard WhiteQueenSideEmpty = (1UL << (int)b1) | (1UL << (int)c1) | (1UL << (int)d1);
    const bitboard BlackKingSideEmpty = (1UL << (int)f8) | (1UL << (int)g8);
    const bitboard BlackQueenSideEmpty = (1UL << (int)b8) | (1UL << (int)c8) | (1UL << (int)d8);
    public Board()
    {
        bitboards = new BitboardSet();
        castlingRights = WhiteKingSide | WhiteQueenSide | BlackKingSide | BlackQueenSide;
        moveHandler = new MoveHandler(this);
        moveValidator = new MoveValidator(this);
        AfterMove = OnUpdate;
        if (!PrecomputedMoveMaps.Generated) PrecomputedMoveMaps.InitAttacks();

    }
    private void OnUpdate(Move move)
    {
        TakeTurn();
        HandleCastlingRights(move.From);
    }
    public bool TryMakeMove(Move move)
    {
        if (moveValidator.IsValid(move))
        {
            moveHandler.MakeMove(move);
            AfterMove?.Invoke(move);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void HandleCastlingRights(int MovedPiecePosition)
    {
        if (castlingRights == None) return;

        castlingRights &= MovedPiecePosition switch
        {
            (int)e1 => ~(WhiteKingSide | WhiteQueenSide),
            (int)e8 => ~(BlackKingSide | BlackQueenSide),
            (int)h1 => ~WhiteKingSide,
            (int)h8 => ~BlackKingSide,
            (int)a1 => ~WhiteQueenSide,
            (int)a8 => ~BlackQueenSide,
            _ => castlingRights
        };
    }
    public void CreatePiece(PieceType pieceType, PieceColor pieceColor, int Position) =>
        bitboards[(int)pieceColor + (int)pieceType] |= 1UL << Position;
    public void TakeTurn()
    {
        if (Turn == white)
            Turn = black;
        else
            Turn = white;
    }
    public PieceColor opTurn()
    {
        if (Turn == white)
            return black;
        else
            return white;
    }
    public void Clear()
    {
        for (int i = 0; i < 12; i++)
        {
            bitboards[i] = 0UL;
        }
    }
    public int GetBitboardIndex(int position)
    {
        for (int i = 0; i < 12; i++)
        {
            if ((bitboards[i] & (1UL << position)) != 0)
                return i;
        }
        return -1;
    }
    public bool IsSquareAttacked(int square)
    {
        PieceColor ColorofAttackers;

        if (Turn == white)
            ColorofAttackers = black;
        else
            ColorofAttackers = white;
        return IsSquareAttacked(square, ColorofAttackers);
    }
    public bool IsSquareAttacked(int square, PieceColor ColorofAttackers)
    {
        for (int i = 0; i < 64; i++)
        {
            for (int k = 0; k < 6; k++)
            {
                int index = k + (int)ColorofAttackers;

                if ((bitboards[index] & (1UL << i)) != 0)
                {
                    MoveMap moveMap = new MoveMap(this, i);
                    moveMap.GenerateAttacks();

                    if ((moveMap.attacks & (1UL << square)) != 0)
                        return true;
                }
            }
        }
        return false;
    }
    public PieceType GetPieceType(int position)
    {
        for (int i = 0; i < 12; i++)
        {
            if ((bitboards[i] & (1UL << position)) != 0)
            {
                return i switch
                {
                    0 => pawn,
                    1 => knight,
                    2 => bishop,
                    3 => rook,
                    4 => queen,
                    5 => king,
                    6 => pawn,
                    7 => knight,
                    8 => bishop,
                    9 => rook,
                    10 => queen,
                    11 => king,
                    _ => empty
                };
            }
        }
        return empty;
    }

    public PieceColor GetPieceColor(int position)
    {
        int BitboardIndex = -1;

        for (int i = 0; i < 12; i++)
        {
            if ((bitboards[i] & (1UL << position)) != 0)
            {
                BitboardIndex = i;
                break;
            }
        }

        if (BitboardIndex < 6 && BitboardIndex > -1) return white;
        if (BitboardIndex > 5 && BitboardIndex > -1) return black;
        return none;
    }
    public bool CanCastle(CastlingRights castlingSide)
    {

        bitboard SideEmpty = castlingSide switch
        {
            WhiteKingSide => WhiteKingSideEmpty,
            WhiteQueenSide => WhiteQueenSideEmpty,
            BlackKingSide => BlackKingSideEmpty,
            BlackQueenSide => BlackQueenSideEmpty,
            _ => 0UL
        };
        PieceColor ClrOfAtck = castlingSide switch
        {
            WhiteKingSide => black,
            WhiteQueenSide => black,
            BlackKingSide => white,
            BlackQueenSide => white,
            _ => none
        };
        bool IsSquaresAttacked = castlingSide switch
        {
            WhiteKingSide => IsSquareAttacked((int)e1, ClrOfAtck) ||
                                IsSquareAttacked((int)f1, ClrOfAtck) ||
                                IsSquareAttacked((int)g1, ClrOfAtck),
            WhiteQueenSide => IsSquareAttacked((int)e1, ClrOfAtck) ||
                               IsSquareAttacked((int)d1, ClrOfAtck) ||
                               IsSquareAttacked((int)c1, ClrOfAtck),
            BlackKingSide => IsSquareAttacked((int)e8, ClrOfAtck) ||
                                IsSquareAttacked((int)f8, ClrOfAtck) ||
                                IsSquareAttacked((int)g8, ClrOfAtck),
            BlackQueenSide => IsSquareAttacked((int)e8, ClrOfAtck) ||
                                IsSquareAttacked((int)d8, ClrOfAtck) ||
                                IsSquareAttacked((int)c8, ClrOfAtck),
            _ => false
        };

        if ((castlingRights & castlingSide) != 0 &&
            (bitboards.Occupied & SideEmpty) == 0 &&
            !IsSquaresAttacked)
            return true;
        else
            return false;
    }
    public int rank(int position) => position / 8;
    public int file(int position) => position % 8;
    public int pos(int rank, int file) => rank * 8 + file;
    public Board Clone()
    {
        Board board = new Board();

        for (int i = 0; i < 12; i++)
            board.bitboards[i] = bitboards[i];

        board.Turn = Turn;

        return board;

    }
}