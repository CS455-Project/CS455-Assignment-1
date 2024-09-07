using MoleProject.Shared;

namespace MoleProject.Pages
{
    public partial class Game {

        private int currentTime = 60;
        public int CurrentTime{
            get { return currentTime; }
            set { currentTime = value; }
        }

        private int gameSpeed = 1000;
        public int GameSpeed{
            get { return gameSpeed; }
            set { gameSpeed = value; }
        }

        private string message = "";
        public string Message{
            get { return message; }
            set { message = value; }
        }

        private int hitPosition = 0;
        public int HitPosition{
            get { return hitPosition; }
            set { hitPosition = value; }
        }

        private bool isGameRunning = true;
        public bool IsGameRunning{
            get { return isGameRunning; }
            set { isGameRunning = value; }
        }
        private int score = 0;
        public int Score{
            get { return score; }
            set { score = value; }
        }


        public List<CellModel> Cells {get;set;} =  new List<CellModel>();

        public Game(){
            for ( int i = 0 ; i < 16 ; i++ ){
                Cells.Add(new CellModel{Id = i});
            }
        }
    }
}

