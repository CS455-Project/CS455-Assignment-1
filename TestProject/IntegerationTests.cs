using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using MoleProject.Shared;
using MoleProject.Pages;
using MoleProject.Layout;

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
            // Use the non-extension method form for setup
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound1", null)).Returns(ValueTask.FromResult((object)null));
            _jsRuntimeMock.Setup(j => j.InvokeAsync<object>("playSound2", null)).Returns(ValueTask.FromResult((object)null));
            Services.AddSingleton(_jsRuntimeMock.Object);
        }

        [Fact]
        public async Task GameLoop_TimersWorkCorrectly()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            var component = cut.Instance;

            // Act
            component.playerName = "TestPlayer";
            component.StartGame();

            // Allow the game to run for a few seconds
            await Task.Delay(3050);

            // Simulate clicking on the active 
            await component.MouseUp(new CellModel { Id = component.hitPosition });

            // End the game
            await component.EndGame();

            // Assert
            Assert.True(component.score > 0, "Score should be greater than 0 after playing");
            Assert.True(component.currentTime < 60, "Time should have decreased");
            Assert.True(component.showGameOverModal, "Game over modal should be shown");

            // Verify that the score was sent to the server
            var leaderboard = await _httpClient.GetFromJsonAsync<List<LeaderboardEntry>>("https://cs455-assignment-1.onrender.com/leaderboard");
            Assert.Contains(leaderboard, entry => entry.Name == "TestPlayer" && entry.Score == component.score);
        }


        // [Fact]
        // public async Task GameEnd_CorrectlyUpdatesLeaderboard()
        // {
        //     // Arrange
        //     var cut = RenderComponent<Game>();
        //     var component = cut.Instance;

        //     string testPlayerName = "TestPlayer" + DateTime.Now.Ticks; // Ensure unique name

        //     // Act
        //     // Start the game
        //     component.playerName = testPlayerName;
        //     component.StartGame();

        //     // Simulate gameplay
        //     for (int i = 0; i < 5; i++) // Simulate 5 successful hits
        //     {
        //         component.score++; // Directly increment score
        //         await component.MouseUp(new CellModel { Id = component.hitPosition }); // Simulate clicking the correct cell
        //     }

        //     // End the game
        //     await component.EndGame();

        //     // Wait a bit for the server to process the update
        //     await Task.Delay(2000);

        //     // Fetch the updated leaderboard
        //     await component.ViewLeaderboard();

        //     // Assert
        //     Assert.True(component.showGameOverModal); // "Game over modal should be shown"
        //     Assert.Equal(5, component.score);// "Score should be 5 after simulating 5 hits"

        //     // Check if the player's score is in the leaderboard
        //     var playerEntry = component.leaderboardData.FirstOrDefault(e => e.Name == testPlayerName);
        //     Assert.NotNull(playerEntry);
        //     Assert.Equal(5, playerEntry.Score);

        //     // Verify the leaderboard is sorted
        //     for (int i = 1; i < component.leaderboardData.Count; i++)
        //     {
        //         Assert.True(component.leaderboardData[i-1].Score >= component.leaderboardData[i].Score,
        //             "Leaderboard should be sorted by score in descending order");
        //     }
        // }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}