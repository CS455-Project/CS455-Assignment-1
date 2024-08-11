namespace MoleProject.Shared
{
    public class SquareModel
    {
        public bool isShown;

        public int Id {get;set;}
<<<<<<< HEAD
        public string? Style{get;set;}
=======
        public string Style{get;set;}
>>>>>>> cd269677a849057d9149e2ff86dce96f17046070
        public bool IsShown{
            get => isShown;
            set {
                isShown = value;
                if ( isShown){
                    Style = "mole";
                }
                else {
                    Style = "";
                }
            }
        }
    }
}