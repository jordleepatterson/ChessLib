using System.Security.Cryptography.X509Certificates;
using chess;

public class ChessHandler : IChessHandler
{
    public Board board { get; private set; }
    public Stockfish stockfish { get; private set; }
    public Bot whiteBot { get; private set; }
    public Bot blackBot { get; private set; }

    public bool WhiteBotEnabled, BlackBotEnabled = false;
    public int depth = 14;
    public string StartPosFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w";
    public void Init()
    {
        board = FEN.LoadFromFen(StartPosFen);

        if (WhiteBotEnabled || BlackBotEnabled)
            stockfish = new Stockfish();

        if (WhiteBotEnabled)
            whiteBot = new Bot(stockfish, board, white);
        if (BlackBotEnabled)
            blackBot = new Bot(stockfish, board, black);

        board.AfterMove += Update;
        stockfish.isReady += OnStockfishReady;
        stockfish.BestMoveFound += OnStockfishMoveFound;

        stockfish.StartEngine();
        PrecomputedMoveMaps.InitAttacks();
    }
    public async void OnStockfishReady()
    {
        Console.WriteLine("asasf");
        //await whiteBot.Play(depth);
    }
    public async void OnStockfishMoveFound(Move move)
    {

    }
    public async void Update(Move move)
    {
        /*if (board.Turn == Piece.piece_color.white)
        {
            await whiteBot.Play(depth);
            return;
        }
        if (board.Turn == Piece.piece_color.black)
        {
            await blackBot.Play(depth);
            return;
        }*/
    }
}