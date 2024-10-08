using Bunit;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System.Threading.Tasks;
using MoleProject.Pages;
using MoleProject.Shared;

namespace GameTests
{
    public class GameTests : TestContext
    {
        private readonly Mock<IJSRuntime> mockJSRuntime;

        public GameTests()
        {
            mockJSRuntime = new Mock<IJSRuntime>();
            Services.AddSingleton(mockJSRuntime.Object);
        }

        [Fact]
        public void Constructor_InitializesCells()
        {
            var game = new Game();

            Assert.Equal(16, game.Cells.Count);
            Assert.All(game.Cells, cell => Assert.IsType<CellModel>(cell));
        }

        [Fact]
        public async Task MouseUp_MissPosition_InvokeJavaScript()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.playerName = "TestPlayer";
            cut.Instance.StartGame();
            var missCell = new CellModel { Id = (cut.Instance.hitPosition + 1) % 16 };

            await cut.Instance.MouseUp(missCell);

            Assert.Equal(0, cut.Instance.score);
            mockJSRuntime.Verify(js => js.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public async Task MouseUp_HitPosition_IncreaseScoreAndInvokeJavaScript()
        {
            var cut = RenderComponent<Game>();
            cut.Instance.playerName = "TestPlayer";
            cut.Instance.StartGame();
            var hitCell = new CellModel { Id = cut.Instance.hitPosition };

            await cut.Instance.MouseUp(hitCell);

            Assert.Equal(1, cut.Instance.score);
            mockJSRuntime.Verify(js => js.InvokeAsync<object>(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

    }
}