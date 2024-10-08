using Bunit;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using MoleProject.Pages;
using MoleProject.Shared;
using Microsoft.JSInterop;  // For IJSRuntime
using HttpHandler;  // For MockHttpMessageHandler

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
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound1", null)).Returns(ValueTask.FromResult((object)null));
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound2", null)).Returns(ValueTask.FromResult((object)null));
            Services.AddSingleton(_jsRuntimeMock.Object);
        }


        [Fact]
        public async Task GameLoop_TimersWorkCorrectly_AndIntegrateWithService()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            var component = cut.Instance;

            // Act
            component.playerName = "TestPlayer";
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(2000);

            for ( int i= 0 ; i < 5; i++)
            {
                // Get the active cell using hitPosition
                var activeCell = component.Cells[component.hitPosition];

                // Trigger MouseUp event on the active cell directly
                await component.MouseUp(activeCell);
            }

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.score == 5, "Score should be 5 after playing");
            Assert.True(component.currentTime < 60, "Time should have decreased");
            Assert.True(component.showGameOverModal, "Game over modal should be shown");
        }
        [Fact]
        public async Task GameEndUpdatesLeaderboardCorrectly()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            var component = cut.Instance;

            // Act
            component.playerName = "TestPlayer";
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(2000);

            // Allot a score 
            component.score = 50;

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.score == 50, "Score should be 5 after playing");
            Assert.True(component.showGameOverModal, "Game over modal should be shown");

            // Verify that the score was sent to the server
            var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>("https://cs455-assignment-1.onrender.com/leaderboard");
            Assert.Contains(leaderboard, entry => entry.Name == "TestPlayer" && entry.Score == component.score);
        }
    }
}
