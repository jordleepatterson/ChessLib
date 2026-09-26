namespace chess;

public readonly record struct Move
{
    public readonly static Move None = new Move(nullSqr, nullSqr);

    public readonly Sqaure From, To;

    public readonly MoveFlags Flag = MoveFlags.None;

    public Move(Sqaure From, Sqaure To)
    {
        this.From = From;
        this.To = To;
    }
    public Move(Sqaure From, Sqaure To, MoveFlags MoveFlag)
    {
        this.From = From;
        this.To = To;
        Flag = MoveFlag;
    }
}