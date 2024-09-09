using Bunit;
using Xunit;

public class WhackEmAllTests : TestContext
{
    [Fact]
    public void StartGame_ShouldInitializeGameCorrectly()
    {
        var gameService = new Game();
        gameService.StartGame();

        Assert.True(gameService.isGameRunning);
        Assert.Equal(0, gameService.score);
        Assert.True(gameService.currentTime >= 0);
    }

    [Fact]
    public void Game_ShouldEndWhenTimeIsZero()
    {
        var gameService = new Game();
    }
}
