using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Retroactiune.Controllers;
using Retroactiune.DataTransferObjects;
using Retroactiune.Models;
using Retroactiune.Services;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.WebAPI.Controllers
{
    public class TestTokensController
    {
        [Fact]
        public async Task Test_GenerateTokens_InexistentFeedbackReceiver()
        {
            // Arrange
            var feedbackService = new Mock<IFeedbackReceiverService>();
            var tokens = new Mock<ITokensService>();

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object);
            var result = await controller.GenerateTokens(new GenerateTokensDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, AutoData]
        public async Task Test_GenerateTokens_Success(FeedbackReceiver randFedFeedbackReceiver)
        {
            // Arrange
            var feedbackService = new Mock<IFeedbackReceiverService>();
            var tokens = new Mock<ITokensService>();

            feedbackService.Setup(i => i.FindAsync(It.IsAny<IEnumerable<string>>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(new[] {randFedFeedbackReceiver});

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object);
            var result = await controller.GenerateTokens(new GenerateTokensDto
            {
                NumberOfTokens = 2,
                FeedbackReceiverId = "froid",
            });

            // Assert
            Assert.IsType<OkObjectResult>(result);
            tokens.Verify(i => i.GenerateTokensAsync(2, "froid", null), Times.Once);
        }
    }
}