using Bunit;
using Xunit;
using System.Threading;

public class WhackEmAllTests : TestContext
{

    [Fact]
<<<<<<< HEAD
    public async Task StartGame_ShouldInitializeGameCorrectly()
{
    var mockGameLoopTimer = new Mock<PeriodicTimer>();
    var mockGameTimeTimer = new Mock<PeriodicTimer>();

    var gameService = new Game();
    gameService.StartGame(); // Replace with injection if necessary

    Assert.Equal(0, gameService.score);
    Assert.Equal(59, gameService.currentTime);

    // Simulate 5 seconds of time passing
    for (int i = 0; i < 5; i++)
    {
        mockGameLoopTimer.Raise(t => t.Tick += null, EventArgs.Empty);
        mockGameTimeTimer.Raise(t => t.Tick += null, EventArgs.Empty);
        await Task.Delay(1000); // Simulate delay for clarity (optional)
=======
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
>>>>>>> cdf04e08446d6d4fa9cbb11f366673da60118f36
    }

    Assert.Equal(54, gameService.currentTime); // Should be 59 - 5
    Assert.True(gameService.isGameRunning);
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
