using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System.Threading.Tasks;
using MoleProject.Pages;
using MoleProject.Shared;

public class GameTests : TestContext
{
    private Mock<IJSRuntime> mockJSRuntime;

    public GameTests()
    {
        mockJSRuntime = new Mock<IJSRuntime>();
        Services.AddSingleton(mockJSRuntime.Object);
    }

    [Fact]
    public void Constructor_InitializesCells()
    {
        // Arrange & Act
        var game = new Game();

        // Assert
        Assert.Equal(16, game.Cells.Count);
        Assert.All(game.Cells, cell => Assert.IsType<CellModel>(cell));
    }

    [Fact]
    public void StartGame_SetsInitialState()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        
        // Act
        cut.Instance.StartGame();

        // Assert
        Assert.True(cut.Instance.isGameStarted);
        Assert.True(cut.Instance.isGameRunning);
        Assert.Equal(0, cut.Instance.score);
        Assert.Equal(60, cut.Instance.currentTime);
        Assert.False(cut.Instance.showGameOverModal);
    }

    [Fact]
    public async Task MouseUp_HitPosition_IncreaseScoreAndPlayHitSound()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        cut.Instance.StartGame();
        var hitCell = new CellModel { Id = cut.Instance.hitPosition };

        // Act
        await cut.Instance.MouseUp(hitCell);

        // Assert
        Assert.Equal(1, cut.Instance.score);
        mockJSRuntime.Verify(js => js.InvokeVoidAsync("playSound1", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public async Task MouseUp_MissPosition_PlayMissSound()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        cut.Instance.StartGame();
        var missCell = new CellModel { Id = (cut.Instance.hitPosition + 1) % 16 };

        // Act
        await cut.Instance.MouseUp(missCell);

        // Assert
        Assert.Equal(0, cut.Instance.score);
        mockJSRuntime.Verify(js => js.InvokeVoidAsync("playSound2", It.IsAny<object[]>()), Times.Once);
    }

    [Fact]
    public void EndGame_SetsCorrectState()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        cut.Instance.StartGame();

        // Act
        cut.Instance.EndGame();

        // Assert
        Assert.False(cut.Instance.isGameRunning);
        Assert.True(cut.Instance.showGameOverModal);
        Assert.Equal("Game Over", cut.Instance.message);
    }

    [Fact]
    public void RestartGame_ResetsGameState()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        cut.Instance.StartGame();
        cut.Instance.EndGame();

        // Act
        cut.Instance.RestartGame();

        // Assert
        Assert.True(cut.Instance.isGameStarted);
        Assert.True(cut.Instance.isGameRunning);
        Assert.Equal(0, cut.Instance.score);
        Assert.Equal(60, cut.Instance.currentTime);
        Assert.False(cut.Instance.showGameOverModal);
    }

    [Fact]
    public void ReturnToStartMenu_SetsCorrectState()
    {
        // Arrange
        var cut = RenderComponent<Game>();
        cut.Instance.StartGame();
        cut.Instance.EndGame();

        // Act
        cut.Instance.ReturnToStartMenu();

        // Assert
        Assert.False(cut.Instance.isGameStarted);
        Assert.False(cut.Instance.showGameOverModal);
    }
}