using System;
using System.Threading.Tasks;

namespace chess;

public class Bot
{
    public PieceColor Side;
    private Stockfish stockfish;
    private Board board;

    public Bot(Stockfish stockfish, Board board, PieceColor Side)
    {
        this.stockfish = stockfish;
        this.board = board;
        this.Side = Side;
    }
    public async Task Play(int depth)
    {
        if (board.Turn == Side)
        {
            Move move = await stockfish.GetBestMoveAsync(FEN.ToFen(board), depth);
            board.TryMakeMove(move);
        }
    }
}