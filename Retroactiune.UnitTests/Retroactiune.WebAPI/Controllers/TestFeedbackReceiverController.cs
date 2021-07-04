using System.Collections.Generic;
using System.Linq;
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
    public class TestFeedbackReceiverController
    {
        [Fact]
        public async Task Post_Fail_Creation_No_items()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.Post(new List<FeedbackReceiverInDto>());

            // Assert, null because we don't have the ApiBehaviourOptions set, which would generate the IActionResult for the invalid input.
            Assert.Null(result);
            mockService.Verify(s => s.CreateManyAsync(It.IsAny<IEnumerable<FeedbackReceiver>>()), Times.Never);
        }

        [Theory, AutoData]
        public async Task Post_Successful_Creation_Two_items(IEnumerable<FeedbackReceiverInDto> items)
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.Post(items);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockService.Verify(s => s.CreateManyAsync(It.IsAny<IEnumerable<FeedbackReceiver>>()), Times.Once);
        }

        [Fact]
        public async Task Delete_Successful()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.Delete("bad_guid_but_unit_test_works_cause_validation_doesnt");

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteManyAsync(new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt"}),
                Times.Once);
            tokensService.Verify(
                s => s.DeleteManyByFeedbackReceiverIdAsync(new[]
                    {"bad_guid_but_unit_test_works_cause_validation_doesnt"}), Times.Once);
        }

        [Fact]
        public async Task DeleteMany_Successful()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var items = new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt", "2", "3"};
            var result = await controller.DeleteMany(items);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteManyAsync(items),
                Times.Once);
            tokensService.Verify(s => s.DeleteManyByFeedbackReceiverIdAsync(items), Times.Once);
        }

        [Fact]
        public async Task DeleteMany_BadRequest()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();
            mockService.Setup(i => i.DeleteManyAsync(It.IsAny<IEnumerable<string>>()))
                .ThrowsAsync(new GenericServiceException("op failed"));

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var items = new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt", "2", "3"};
            var result = await controller.DeleteMany(items);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockService.Verify(s => s.DeleteManyAsync(items), Times.Once);
            tokensService.Verify(s => s.DeleteManyByFeedbackReceiverIdAsync(items), Times.Once);
        }

        [Fact]
        public async Task Get_Successful()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();
            mockService.Setup(i => i.FindAsync(It.IsAny<IEnumerable<string>>(), null, null))
                .ReturnsAsync(new[] {new FeedbackReceiver()});

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.Get("bad_guid_but_unit_test_works_cause_validation_doesnt");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockService.Verify(
                s => s.FindAsync(new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt"}, null, null),
                Times.Once);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.Get("bad_guid_but_unit_test_works_cause_validation_doesnt");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockService.Verify(
                s => s.FindAsync(new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt"}, null, null),
                Times.Once);
        }

        [Theory, AutoData]
        public async Task List_Ok(IEnumerable<string> filter, int offset, int limit)
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiversService>();
            var tokensService = new Mock<ITokensService>();
            var feedbacksService = new Mock<IFeedbacksService>();
            var logger = new Mock<ILogger<FeedbackReceiversController>>();
            var filterArr = filter as string[] ?? filter.ToArray();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, tokensService.Object,
                feedbacksService.Object, mapper, null,
                logger.Object);
            var result = await controller.List(filterArr, offset, limit);

            Assert.IsType<OkObjectResult>(result);
            mockService.Verify(s => s.FindAsync(filterArr, offset, limit), Times.Once);
        }
    }
}