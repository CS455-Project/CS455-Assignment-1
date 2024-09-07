using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game
    {
        public int currentTime { get; set; } = 60;
        public int gameSpeed { get; set; } = 750;
        public string message { get; set; }
        public int hitPosition { get; set; } = 0;
        public bool isGameRunning { get; set; }
        public int score { get; set; } = 0;
        public List<CellModel> Cells { get; set; } = new List<CellModel>();

        public Game()
        {
            for (int i = 0; i < 16; i++)
            {
                Cells.Add(new CellModel { Id = i });
            }
        }
    }
}
