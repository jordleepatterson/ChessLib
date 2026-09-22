namespace chess;

public struct Piece
{
    public readonly PieceType pieceType;
    public readonly PieceColor pieceColor;
    public int Position;

    public Piece(PieceType pieceType, PieceColor pieceColor, int Position)
    {
        this.pieceType = pieceType;
        this.pieceColor = pieceColor;
        this.Position = Position;
    }

}
public enum PieceColor
{
    white = 0,
    black = 6,
    none = -1
}
public enum PieceType
{
    pawn = 0,
    knight = 1,
    bishop = 2,
    rook = 3,
    queen = 4,
    king = 5,
    empty = -1
}