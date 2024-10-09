using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game
    {
        public static string ServerUrl { get; } = "https://cs455-assignment-1.onrender.com";

        public GameState State { get; set; } = new GameState();
        public GameConfig Config { get; set; } = new GameConfig();
        public Player CurrentPlayer { get; set; } = new Player();
        public LeaderboardManager CurrLeaderboardManager { get; set; } = new LeaderboardManager();
        public CellManager CurrCellManager { get; set; } = new CellManager();

        public Game() { }
    }

    public class GameState
    {
        public int CurrentTime { get; set; } = 60;
        public string Message { get; set; } = string.Empty;
        public int HitPosition { get; set; } = 0;
        public bool IsGameRunning { get; set; }
        public bool IsGameStarted { get; set; }
        public bool ShowGameOverModal { get; set; }
        public int? LastPosition { get; set; }
    }

    public class GameConfig
    {
        public int GameSpeed { get; set; } = 750;
        public PeriodicTimer? GameLoopTimer { get; set; }
        public PeriodicTimer? GameTimeTimer { get; set; }
    }

    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    public class LeaderboardManager
    {
        public bool ShowLeaderboard { get; set; }
        public bool ShowNamePrompt { get; set; }
        public List<LeaderboardEntry> Entries { get; set;} = new List<LeaderboardEntry>();
    }

    public class CellManager
    {
        private const int count = 16;
        public List<CellModel> Cells { get; set;}

        public CellManager()
        {
            var cells = new List<CellModel>();
            for (int i = 0; i < count; i++)
            {
                cells.Add(new CellModel { Id = i });
            }
            Cells = cells;
        }

    }

    public class LeaderboardEntry
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}