using Bunit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using MoleProject.Shared;
using Microsoft.JSInterop;  // For IJSRuntime
using HttpHandler;  // For MockHttpMessageHandler
using RichardSzalay.MockHttp;
using MoleProject.Pages;


namespace WhackEmAllTests
{
    public class WhackEmAllIntegrationTests : TestContext
    {
        private readonly HttpClient _httpClient;
        private readonly Mock<IJSRuntime> _jsRuntimeMock;

        public WhackEmAllIntegrationTests()
        {
            _httpClient = new HttpClient();
            Services.AddSingleton(_httpClient);

            _jsRuntimeMock = new Mock<IJSRuntime>();
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound1", null)).ReturnsAsync((object?)null);
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound2", null)).ReturnsAsync((object?)null);
            Services.AddSingleton(_jsRuntimeMock.Object);
        }


        [Fact]
        public async Task GameLoop_TimersWorkCorrectly_AndIntegrateWithService()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            var component = cut.Instance;

            // Act
            component.CurrentPlayer.Name = "TestPlayer";
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(2000);

            for (int i = 0; i < 5; i++)
            {
                // Get the active cell using hitPosition
                var activeCell = component.CurrCellManager.Cells[component.State.HitPosition];

                // Trigger MouseUp event on the active cell directly
                await component.MouseUp(activeCell);
            }

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.CurrentPlayer.Score == 5, "Score should be 5 after playing");
            Assert.True(component.State.CurrentTime < 60, "Time should have decreased");
            Assert.True(component.State.ShowGameOverModal, "Game over modal should be shown");
        }
        [Fact]
        public async Task GameEndUpdatesLeaderboardCorrectly()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            var component = cut.Instance;

            // Act
            component.CurrentPlayer.Name = "TestPlayer";
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(2000);

            // Allot a score 
            component.CurrentPlayer.Score = 50;

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.CurrentPlayer.Score == 50, "Score should be 5 after playing");
            Assert.True(component.State.ShowGameOverModal, "Game over modal should be shown");

            // Verify that the score was sent to the server
            var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>($"{Game.ServerUrl}/leaderboard");
            Assert.Contains(leaderboard!, entry => entry.Name == "TestPlayer" && entry.Score == component.CurrentPlayer.Score);
        }

        [Fact]
        public async Task ViewLeaderboard_FetchesAndDisplaysLeaderboard()
        {
            // Arrange
            var mockHttp = new RichardSzalay.MockHttp.MockHttpMessageHandler();
            var leaderboardData = new[]
            {
                new { Name = "Player1", Score = 100 },
                new { Name = "Player2", Score = 90 }
            };

            mockHttp.When($"{Game.ServerUrl}/leaderboard")
                    .Respond("application/json", System.Text.Json.JsonSerializer.Serialize(leaderboardData));

            var client = mockHttp.ToHttpClient();
            Services.AddSingleton(client);

            var cut = RenderComponent<Game>();

            // Act
            await cut.Instance.ViewLeaderboard();

            // Assert
            Assert.True(cut.Instance.CurrLeaderboardManager.ShowLeaderboard);
            Assert.Equal(2, cut.Instance.CurrLeaderboardManager.Entries.Count);
            Assert.False(cut.Instance.State.IsGameStarted);
            Assert.False(cut.Instance.State.ShowGameOverModal);

            // Additional assertions to check the content of leaderboardData
            Assert.Equal("Player1", cut.Instance.CurrLeaderboardManager.Entries[0].Name);
            Assert.Equal(100, cut.Instance.CurrLeaderboardManager.Entries[0].Score);
            Assert.Equal("Player2", cut.Instance.CurrLeaderboardManager.Entries[1].Name);
            Assert.Equal(90, cut.Instance.CurrLeaderboardManager.Entries[1].Score);
        }
    }
}