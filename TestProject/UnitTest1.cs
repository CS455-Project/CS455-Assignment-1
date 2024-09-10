using Bunit;
using Xunit;

public class WhackEmAllTests : TestContext
{

    [Fact]
    public void StartGame_ShouldInitializeGameCorrectly()
    {
        var gameService = new Game();
        gameService.StartGame();
        // Assert.Equal(60, gameService.currentTime);

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
        var gameService = new Game();
        gameService.EndGame();

        Assert.False(gameService.isGameRunning);
        Assert.True(gameService.showGameOverModal);
        Assert.Equal("Game Over", gameService.message);
    }
}
