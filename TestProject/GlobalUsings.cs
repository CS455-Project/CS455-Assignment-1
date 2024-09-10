global using Xunit;
global using MoleProject;
global using MoleProject.Layout;
global using MoleProject.Pages;
global using MoleProject.Shared;

public class TestableGame : Game
{
    protected override async Task GameLoopAsync(PeriodicTimer timer)
    {
        // while (isGameRunning)
        // {
        //     setNextAppearance();
        //     await timer.WaitForNextTickAsync();
        // }
    }

    protected override async Task GameTimeAsync(PeriodicTimer timer)
    {
        // while (isGameRunning)
        // {

        //     if (currentTime == 0)
        //     {
        //         EndGame();
        //         break;
        //     }

        //     StateHasChanged();
        //     await timer.WaitForNextTickAsync();
        //     currentTime--;
        // }
    }
}