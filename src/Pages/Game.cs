using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game
    {
        public int currentTime { get; set; }
        public int gameSpeed { get; set; } = 750;
        public string message { get; set; } = "";
        public int hitPosition { get; set; } = 0;
        public bool isGameRunning { get; set; } = false;
        public bool isGameStarted { get; set; } = false;
        public bool showGameOverModal { get; set; } = false;
        public int score { get; set; } = 0;
        public PeriodicTimer? gameLoopTimer { get; set; }
        public PeriodicTimer? gameTimeTimer { get; set; }
        public List<CellModel> Cells { get; set; } = new List<CellModel>();
        public string playerName {get; set;}= string.Empty;
        public int? lastPosition = null;
        public string serverUrl {get;set;} = "https://cs455-assignment-1.onrender.com";
        public bool showLeaderboard {get;set;} = false;
        public bool showNamePrompt{get;set;} = false;

        public Game()
        {
            for (int i = 0; i < 16; i++)
            {
                Cells.Add(new CellModel { Id = i });
            }
        }
    }
    public class LeaderboardEntry
    {
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
