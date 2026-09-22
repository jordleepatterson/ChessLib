namespace chess;

public struct BitboardSet
{
    private bitboard[] bitboards;
    public BitboardSet() => bitboards = new bitboard[12];

    public ref bitboard wPawns => ref bitboards[0];
    public ref bitboard wKnights => ref bitboards[1];
    public ref bitboard wBishops => ref bitboards[2];
    public ref bitboard wRooks => ref bitboards[3];
    public ref bitboard wQueens => ref bitboards[4];
    public ref bitboard wKing => ref bitboards[5];
    public ref bitboard bPawns => ref bitboards[6];
    public ref bitboard bKnights => ref bitboards[7];
    public ref bitboard bBishops => ref bitboards[8];
    public ref bitboard bRooks => ref bitboards[9];
    public ref bitboard bQueens => ref bitboards[10];
    public ref bitboard bKing => ref bitboards[11];

    public bitboard wPieces => wPawns |
                                    wKnights |
                                    wBishops |
                                    wRooks |
                                    wQueens |
                                    wKing;
    public bitboard bPieces => bPawns |
                                    bKnights |
                                    bBishops |
                                    bRooks |
                                    bQueens |
                                    bKing;
    public bitboard Occupied => wPieces | bPieces;

    public bitboard this[int index]
    {
        get => bitboards[index];
        set => bitboards[index] = value;
    }
    public bitboard this[BitboardEnum bitboard]
    {
        get => bitboards[(int)bitboard];
        set => bitboards[(int)bitboard] = value;
    }
}
public enum BitboardEnum
{
    whitePawns, whiteKnights, whiteBishops, whiteRooks, whiteQueens, whiteKing,
    blackPawns, blackKnights, blackBishops, blackRooks, blackQueens, blackKing,
}