using System.Diagnostics;
namespace chess;

public class Stockfish
{
    private Process stockfish;
    private StreamWriter input;
    public StreamReader output;

    public event Action<Move> BestMoveFound;
    public event Action isReady;
    public bool Started { get; private set; } = false;

    private readonly object stockfishLock = new();

    public static string stockfishPath = "stockfish.exe";

    public void StartEngine()
    {
        stockfish = new Process();

        stockfish.StartInfo.FileName = stockfishPath;
        stockfish.StartInfo.UseShellExecute = false;
        stockfish.StartInfo.RedirectStandardInput = true;
        stockfish.StartInfo.RedirectStandardOutput = true;
        stockfish.StartInfo.CreateNoWindow = true;

        stockfish.Start();

        input = stockfish.StandardInput;
        output = stockfish.StandardOutput;

        input.AutoFlush = true;

        InitializeEngine();
    }

    private void InitializeEngine()
    {
        input.WriteLine("uci");
        WaitFor("uciok");
        input.WriteLine("setoption name UCI_LimitStrength value true");
        input.WriteLine("setoption name UCI_Elo value 3200");
        input.WriteLine("isready");
        WaitFor("readyok");


        isReady?.Invoke();
        Started = true;
    }
    public void ShutdownEngine()
    {
        try
        {
            if (stockfish != null && !stockfish.HasExited)
            {
                input.WriteLine("quit");
                stockfish.WaitForExit();
                stockfish.Dispose();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error shutting down Stockfish: {e.Message}");
        }
    }

    private void WaitFor(string token)
    {
        string line;

        while ((line = output.ReadLine()) != null)
        {
            if (line.Contains(token))
                return;
        }
    }
    public async Task<Move> GetBestMoveAsync(string fenString, int depth)
    {
        return await Task.Run(() =>
        {
            lock (stockfishLock)
            {
                try
                {
                    input.WriteLine($"position fen {fenString}");
                    input.WriteLine($"go depth {depth}");

                    string line;
                    string[] fenStringParts = fenString.Split(' ');

                    while ((line = output.ReadLine()) != null)
                    {
                        if (line.StartsWith("bestmove"))
                        {
                            string[] parts = line.Split(' ');

                            if (parts.Length >= 2)
                            {
                                string movestr = parts[1];
                                string From = movestr.Substring(0, 2);
                                string To = movestr.Substring(2, 2);

                                int FromIndex = 0;
                                int ToIndex = 0;
                                for (int i = 0; i < 64; i++)
                                {
                                    if (Board.CharBoard[i] == From) FromIndex = i;
                                    if (Board.CharBoard[i] == To) ToIndex = i;
                                }
                                Move move = new Move(FromIndex, ToIndex);

                                BestMoveFound?.Invoke(move);
                                return move;
                            }

                            break;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Stockfish error: {e.Message}");
                }

                return Move.None;
            }
        });
    }
}