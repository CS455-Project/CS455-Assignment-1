using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game {
        public int score = 0;
        public int currentTime = 15;
        public int gameSpeed = 500;
        public string msg = "new game has begun";
        public int hitPosition = 0;
        public bool isGameRunning = true;

        public List<SquareModel> Squares {get;set;} =  new List<SquareModel>();

        public Game(){
            for ( int i = 0 ; i < 9 ; i++ ){
                Squares.Add(new SquareModel{Id = i});
            }
        }
    }
}