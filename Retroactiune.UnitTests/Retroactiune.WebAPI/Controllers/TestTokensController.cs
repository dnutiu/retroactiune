using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Retroactiune.Controllers;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.WebAPI.Controllers
{
    public class TestTokensController
    {
        [Fact]
        public async Task Test_GenerateTokens_InexistentFeedbackReceiver()
        {
            // Arrange
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var mapper = TestUtils.GetMapper();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.GenerateTokens(new GenerateTokensDto());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory, AutoData]
        public async Task Test_GenerateTokens_Success(FeedbackReceiver randFedFeedbackReceiver)
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();

            feedbackService.Setup(i => i.FindAsync(It.IsAny<IEnumerable<string>>(),
                    It.IsAny<int?>(),
                    It.IsAny<int?>()))
                .ReturnsAsync(new[] {randFedFeedbackReceiver});

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.GenerateTokens(new GenerateTokensDto
            {
                NumberOfTokens = 2,
                FeedbackReceiverId = "froid",
            });

            // Assert
            Assert.IsType<OkObjectResult>(result);
            tokens.Verify(i => i.GenerateTokensAsync(2, "froid", null), Times.Once);
        }

        [Fact]
        public async Task Test_Delete_Ok()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.DeleteToken("my_guid");

            // Assert
            Assert.IsType<NoContentResult>(result);
            tokens.Verify(i => i.DeleteTokensAsync(new[] {"my_guid"}), Times.Once);
        }

        [Fact]
        public async Task Test_Delete_BadRequest()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();
            tokens.Setup(i => i.DeleteTokensAsync(It.IsAny<IEnumerable<string>>()))
                .Throws(new GenericServiceException("op fail"));

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.DeleteToken("my_guid");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            tokens.Verify(i => i.DeleteTokensAsync(new[] {"my_guid"}), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteMany_Ok()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.DeleteTokens(new[] {"my_guid", "b"});

            // Assert
            Assert.IsType<NoContentResult>(result);
            tokens.Verify(i => i.DeleteTokensAsync(new[] {"my_guid", "b"}), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteMany_BadRequest()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();
            tokens.Setup(i => i.DeleteTokensAsync(It.IsAny<IEnumerable<string>>()))
                .Throws(new GenericServiceException("op fail"));

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.DeleteTokens(new[] {"my_guid", "b"});

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            tokens.Verify(i => i.DeleteTokensAsync(new[] {"my_guid", "b"}), Times.Once);
        }

        [Fact]
        public async Task Test_ListTokens_Ok()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.ListTokens(null);
            
            // Assert
            Assert.IsType<OkObjectResult>(result);
            tokens.Verify(i => i.FindAsync(It.IsAny<TokenListFilters>()), Times.Once);
        }
        
        [Fact]
        public async Task Test_ListTokens_BadRequest()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();
            tokens.Setup(i => i.FindAsync(It.IsAny<TokenListFilters>()))
                .Throws(new GenericServiceException("op fail"));

            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.ListTokens(null);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            tokens.Verify(i => i.FindAsync(It.IsAny<TokenListFilters>()), Times.Once);
        }

        [Fact]
        public async Task Test_CheckToken_NotFound()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();
            tokens.Setup(i => i.FindAsync(It.IsAny<TokenListFilters>()))
                .ReturnsAsync(new List<Token>());
            
            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.CheckToken("random");
            
            // Assert
            var checkResult = (CheckTokenDto) ((ObjectResult) result).Value;
            Assert.IsType<OkObjectResult>(result);
            Assert.False(checkResult.IsValid);
        }
        
        [Fact]
        public async Task Test_CheckToken_Valid()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var feedbackService = new Mock<IFeedbackReceiversService>();
            var tokens = new Mock<ITokensService>();
            var logger = new Mock<ILogger<TokensController>>();
            tokens.Setup(i => i.FindAsync(It.IsAny<TokenListFilters>()))
                .ReturnsAsync(new List<Token> { new Token() });
            
            // Test
            var controller = new TokensController(feedbackService.Object, tokens.Object, logger.Object, mapper);
            var result = await controller.CheckToken("random");
            
            // Assert
            var checkResult = (CheckTokenDto) ((ObjectResult) result).Value;
            Assert.IsType<OkObjectResult>(result);
            Assert.True(checkResult.IsValid);
        }
    }
}