using Bunit;
using Xunit;
using MoleProject.Pages;
using Moq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq.Protected;
using Newtonsoft.Json;

namespace WhackEmAllTests
{
    public class WhackEmAllTests : TestContext
    {
        [Fact]
        public void StartGame_ShouldInitializeGameCorrectly()
        {
            var gameService = new Game
            {
                playerName = "TestPlayer"
            };
            gameService.StartGame();


            Assert.Equal(0, gameService.score);
            Assert.Equal(750, gameService.gameSpeed);
            Assert.Equal(16, gameService.Cells.Count);
            Assert.True(gameService.isGameStarted);
            Assert.True(string.IsNullOrEmpty(gameService.message));
            Assert.False(gameService.showGameOverModal);
            Assert.NotNull(gameService.gameLoopTimer);
            Assert.NotNull(gameService.gameTimeTimer);
            Assert.Equal(60, gameService.currentTime);
            Assert.True(gameService.isGameRunning);
        }

        [Fact]
        public async Task EndGame_ShouldTerminateGameCorrectly()
        {
            // Arrange
            var mockGame = new Mock<Game>
            {
                CallBase = true // Ensures the actual logic is used except for mocked methods
            };
            mockGame.Object.playerName = "TestPlayer";
            mockGame.Object.StartGame();
            mockGame.Setup(x => x.SendScoreToServer(It.IsAny<int>())).Returns(Task.CompletedTask); // Mock the method

            // Act
            await mockGame.Object.EndGame();

            // Assert
            Assert.False(mockGame.Object.isGameRunning);
            Assert.True(mockGame.Object.showGameOverModal);
            mockGame.Verify(x => x.SendScoreToServer(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void SetNextAppearance_ShouldSetNextMoleAppearanceCorrectly()
        {
            var gameService = new Game
            {
                playerName = "TestPlayer"
            };

            gameService.StartGame();

            gameService.setNextAppearance();
            int currentMoleIndex = gameService.hitPosition;
            Assert.True(gameService.Cells[currentMoleIndex].IsShown);

            gameService.setNextAppearance();
            int newMoleIndex = gameService.hitPosition;

            Assert.False(gameService.Cells[currentMoleIndex].IsShown);
            Assert.True(gameService.Cells[newMoleIndex].IsShown);
        }

        [Fact]
        public void RestartGame_ResetsGameState()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.StartGame();
            cut.Instance.playerName = "TestPlayer";
            cut.Instance.EndGame();

            cut.Instance.RestartGame();

            Assert.True(cut.Instance.isGameStarted);
            Assert.True(cut.Instance.isGameRunning);
            Assert.Equal(0, cut.Instance.score);
            Assert.Equal(60, cut.Instance.currentTime);
            Assert.False(cut.Instance.showGameOverModal);
        }

        [Fact]
        public void ReturnToStartMenu_SetsCorrectState()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.StartGame();
            cut.Instance.playerName = "TestPlayer";
            cut.Instance.EndGame();

            cut.Instance.ReturnToStartMenu();

            Assert.False(cut.Instance.isGameStarted);
            Assert.False(cut.Instance.showGameOverModal);
        }

    }
}

