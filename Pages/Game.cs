using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game {
        public int score = 0;
        public int currentTime = 30;
        public int gameSpeed = 1000;
        public string msg = "";
        public int hitPosition = 0;
        public bool isGameRunning = true;

        public List<SquareModel> Squares {get;set;} =  new List<SquareModel>();

        public Game(){
            for ( int i = 0 ; i < 16 ; i++ ){
                Squares.Add(new SquareModel{Id = i});
                // System.Console.WriteLine(i);
            }
        }
    }
}