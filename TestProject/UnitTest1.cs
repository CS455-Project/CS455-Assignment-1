using Bunit;
using Xunit;

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
public class WhackEmAllTests : TestContext
{

    [Fact]
    public void StartGame_ShouldInitializeGameCorrectly()
    {
        var gameService = new TestableGame();
        gameService.StartGame();

        Assert.True(gameService.isGameRunning);
        Assert.Equal(0, gameService.score);
        Assert.Equal(750, gameService.gameSpeed);
        Assert.Equal(16, gameService.Cells.Count);
        Assert.True(gameService.isGameStarted);
        Assert.True(string.IsNullOrEmpty(gameService.message));
        Assert.False(gameService.showGameOverModal);
        Assert.NotNull(gameService.gameLoopTimer);
        Assert.NotNull(gameService.gameTimeTimer); 
        Assert.Equal(60, gameService.currentTime);
    }

    [Fact]
    public void EndGame_ShouldTerminateGameCorrectly()
    {
        var gameService = new TestableGame();
        gameService.EndGame();

        Assert.False(gameService.isGameRunning);
        Assert.True(gameService.showGameOverModal);
        Assert.Equal("Game Over", gameService.message);
    }

    [Fact]
    public void SetNextAppearance_ShouldSetNextMoleAppearanceCorrectly()
    {
        var gameService = new TestableGame();

        gameService.StartGame();

        gameService.setNextAppearance();
        int currentMoleIndex = gameService.hitPosition;
        Assert.True(gameService.Cells[currentMoleIndex].IsShown);

        gameService.setNextAppearance();
        int newMoleIndex = gameService.hitPosition;

        Assert.False(gameService.Cells[currentMoleIndex].IsShown);
        Assert.True(gameService.Cells[newMoleIndex].IsShown);
    }
}
