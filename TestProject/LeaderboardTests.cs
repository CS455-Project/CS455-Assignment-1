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

            Assert.True(cut.Instance.showNamePrompt);
        }

        [Fact]
        public void CancelNamePrompt_ResetsNamePromptState()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.ShowNamePrompt();
            cut.Instance.playerName = "TestPlayer";

            cut.Instance.CancelNamePrompt();

            Assert.False(cut.Instance.showNamePrompt);
            Assert.Equal(string.Empty, cut.Instance.playerName);
        }

        [Fact]
        public async Task ViewLeaderboard_SendsGetRequest()
{
    // Arrange
    var mockHttp = new MockHttpMessageHandler();
    var requestMade = false;

    mockHttp.When("https://cs455-assignment-1.onrender.com/leaderboard")
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
    Assert.False(cut.Instance.isGameStarted);
    Assert.False(cut.Instance.showGameOverModal);
}

        [Fact]
        public void CloseLeaderboard_HidesLeaderboard()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.showLeaderboard = true;

            cut.Instance.CloseLeaderboard();

            Assert.False(cut.Instance.showLeaderboard);
        }

        [Fact]
        public void StartGame_WithValidName_StartsGameAndHidesNamePrompt()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            cut.Instance.playerName = "ValidPlayer";
            cut.Instance.showNamePrompt = true;

            // Act
            cut.Instance.StartGame();

            // Assert
            Assert.True(cut.Instance.isGameStarted);
            Assert.False(cut.Instance.showNamePrompt);
            Assert.Equal(0, cut.Instance.score);
            Assert.Equal(60, cut.Instance.currentTime);
            Assert.True(cut.Instance.isGameRunning);
            Assert.False(cut.Instance.showGameOverModal);
        }

        [Fact]
        public void StartGame_WithInvalidName_DoesNotStartGame()
        {
            // Arrange
            var cut = RenderComponent<Game>();
            cut.Instance.playerName = "";
            cut.Instance.showNamePrompt = true;

            // Act
            cut.Instance.StartGame();

            // Assert
            Assert.False(cut.Instance.isGameStarted);
            Assert.True(cut.Instance.showNamePrompt);
        }
    }
}