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
using Microsoft.VisualBasic;


namespace WhackEmAllTests
{
    public class WhackEmAllIntegrationTests : TestContext
    {
        public static string GenerateRandomName() {
            return $"TestPlayer_{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 4)}";
        }
        private readonly Random randomScore = new Random();
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
            component.CurrentPlayer.Name = GenerateRandomName();
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
            var name = GenerateRandomName();
            component.CurrentPlayer.Name = name;
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(2000);

            // Allot a score 
            var score = randomScore.Next(200, 400);
            component.CurrentPlayer.Score = score;

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.CurrentPlayer.Score == score, "Score should be correct after playing");
            Assert.True(component.State.ShowGameOverModal, "Game over modal should be shown");

            // Verify that the score was sent to the server
            var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>($"{Game.ServerUrl}/leaderboard");
            Assert.Contains(leaderboard!, entry => entry.Name == name && entry.Score == component.CurrentPlayer.Score);

            // Cleanup: Remove the entry from the leaderboard
            var url = $"{Game.ServerUrl}/leaderboard/delete?name={Uri.EscapeDataString(name)}";
            var response = await _httpClient.PostAsync(url, null);
            Assert.True(response.IsSuccessStatusCode, "Entity should be deleted successfully");
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