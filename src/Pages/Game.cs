using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game
    {
        private const int InitialGameSpeed = 750;
        private const int CellCount = 16;

        public static string ServerUrl { get; } = "https://cs455-assignment-1.onrender.com";

        public GameState State { get; set;} = new GameState();
        public GameConfig Config { get; set;} = new GameConfig();
        public Player CurrentPlayer { get; set;} = new Player();
        public LeaderboardManager CurrLeaderboardManager { get; set;} = new LeaderboardManager();
        public CellManager CurrCellManager { get; set;} = new CellManager(CellCount);

        public Game()
        {
            Config.GameSpeed = InitialGameSpeed;
        }
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
        public int GameSpeed { get; set; }
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
        public List<CellModel> Cells { get; set; }

        public CellManager(int cellCount)
        {
            var cells = new List<CellModel>();
            for (int i = 0; i < cellCount; i++)
            {
                cells.Add(new CellModel { Id = i });
            }
            Cells = new List<CellModel>(cells);
        }
    }

    public class LeaderboardEntry
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}