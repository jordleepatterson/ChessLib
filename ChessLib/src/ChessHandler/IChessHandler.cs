using chess;

public interface IChessHandler
{
    public Board board{get;}
    public Stockfish stockfish{get;}
    public void Init();
    public void Update(Move move);
    public void OnStockfishMoveFound(Move move);
    public void OnStockfishReady();
}