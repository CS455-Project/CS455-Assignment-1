using Bunit;
using Xunit;

public class WhackEmAllTests : TestContext
{
    [Fact]
    public void StartGame_ShouldInitializeGameCorrectly()
    {
        // Arrange: Render the component and provide the necessary service.
        var gameService = new Game();
        gameService.StartGame();
        // Services.AddSingleton(gameService); // Add the GameService to the test DI container

        // var cut = RenderComponent<WhackEmAll>(); // Replace with the actual component name

        // Act: Simulate the click on the Start button.
        // cut.Find("button").Click(); // Assuming there's only one button (Start Game)

        // Assert: Check if the game started correctly.

        Assert.True(gameService.isGameRunning);
        Assert.Equal(0, gameService.score);
        Assert.True(gameService.currentTime >= 0);
    }

    [Fact]
    public void Game_ShouldEndWhenTimeIsZero()
    {
        // Arrange: Render the component and inject the game service.
        var gameService = new Game();
        // Services.AddSingleton(gameService);

        // var cut = RenderComponent<WhackEmAll>(); 
    }
}
