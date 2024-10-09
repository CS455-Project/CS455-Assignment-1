using Bunit;
using Xunit;
using MoleProject.Pages;
using Moq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace WhackEmAllTests
{
    public class WhackEmAllAdditionalTests : TestContext
    {
        [Fact]
        public void ShowNamePrompt_SetsCorrectState()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.ShowNamePrompt();

            Assert.True(cut.Instance.CurrLeaderboardManager.ShowNamePrompt);
        }

        [Fact]
        public void CancelNamePrompt_ResetsNamePromptState()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.ShowNamePrompt();
            cut.Instance.CurrentPlayer.Name = "TestPlayer";

            cut.Instance.CancelNamePrompt();

            Assert.False(cut.Instance.CurrLeaderboardManager.ShowNamePrompt);
            Assert.Equal(string.Empty, cut.Instance.CurrentPlayer.Name);
        }

        [Fact]
        public async Task ViewLeaderboard_SendsGetRequest()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var requestMade = false;

            mockHttp.When($"{Game.ServerUrl}/leaderboard")
                    .Respond(req =>
                    {
                        requestMade = true;
                        return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                    });

            var client = mockHttp.ToHttpClient();
            Services.AddSingleton(client);

            var cut = RenderComponent<Game>();

            // Act
            await cut.Instance.ViewLeaderboard();

            // Assert
            Assert.True(requestMade, "GET request was not sent to the leaderboard endpoint");
            Assert.False(cut.Instance.State.IsGameStarted);
            Assert.False(cut.Instance.State.ShowGameOverModal);
        }

        [Fact]
        public void CloseLeaderboard_HidesLeaderboard()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.CurrLeaderboardManager.ShowLeaderboard = true;

            cut.Instance.CloseLeaderboard();

            Assert.False(cut.Instance.CurrLeaderboardManager.ShowLeaderboard);
        }

        [Fact]
        public void StartGame_WithValidName_StartsGameAndHidesNamePrompt()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            cut.Instance.CurrentPlayer.Name = "ValidPlayer";
            cut.Instance.CurrLeaderboardManager.ShowNamePrompt = true;

            // Act
            cut.Instance.StartGame();

            // Assert
            Assert.True(cut.Instance.State.IsGameStarted);
            Assert.False(cut.Instance.CurrLeaderboardManager.ShowNamePrompt);
            Assert.Equal(0, cut.Instance.CurrentPlayer.Score);
            Assert.Equal(60, cut.Instance.State.CurrentTime);
            Assert.True(cut.Instance.State.IsGameRunning);
            Assert.False(cut.Instance.State.ShowGameOverModal);
        }

        [Fact]
        public void StartGame_WithInvalidName_DoesNotStartGame()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            cut.Instance.CurrentPlayer.Name = "";
            cut.Instance.CurrLeaderboardManager.ShowNamePrompt = true;

            // Act
            cut.Instance.StartGame();

            // Assert
            Assert.False(cut.Instance.State.IsGameStarted);
            Assert.True(cut.Instance.CurrLeaderboardManager.ShowNamePrompt);
        }
    }
}