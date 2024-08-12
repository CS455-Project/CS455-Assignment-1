using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game {
        public int score = 0;
        public int currentTime = 60;
        public int gameSpeed = 500;
        public string msg = "";
        public int hitPosition = 0;
        public bool isGameRunning = true;

        public List<CellModel> Cells {get;set;} =  new List<CellModel>();

        public Game(){
            for ( int i = 0 ; i < 16 ; i++ ){
                Cells.Add(new CellModel{Id = i});
                // System.Console.WriteLine(i);
            }
        }
    }
}