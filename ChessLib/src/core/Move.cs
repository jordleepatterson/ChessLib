namespace chess;

[Flags]
public enum MoveFlags : byte
{
    None = 0,
    Capture = 1 << 0,
    DoublePawnPush = 1 << 1,
    EnPassant = 1 << 2,
    CastleKingSide = 1 << 3,
    CastleQueenSide = 1 << 4,
    Promotion = 1 << 5,
    Normal = 1 << 6,
    Castle = 1 << 7
}
public readonly record struct Move
{
    public readonly static Move None = new Move(-1, -1);

    public readonly int From, To;

    public readonly MoveFlags Flag = MoveFlags.None;

    public Move(int From, int To)
    {
        this.From = From;
        this.To = To;
    }
    public Move(int From, int To, MoveFlags MoveFlag)
    {
        this.From = From;
        this.To = To;
        Flag = MoveFlag;
    }
}