using Bunit;
using Xunit;
using System.Threading;

public class WhackEmAllTests : TestContext
{
    [Fact]
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
    }

    Assert.Equal(54, gameService.currentTime); // Should be 59 - 5
    Assert.True(gameService.isGameRunning);
}

    [Fact]
    public void Game_ShouldEndWhenTimeIsZero()
    {
        var gameService = new Game();
    }
}
