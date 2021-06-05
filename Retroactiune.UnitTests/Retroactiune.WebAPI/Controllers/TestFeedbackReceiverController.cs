using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Retroactiune.Controllers;
using Retroactiune.Models;
using Retroactiune.Services;
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
            var mockService = new Mock<IFeedbackReceiverService>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
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
            var mockService = new Mock<IFeedbackReceiverService>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
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
            var mockService = new Mock<IFeedbackReceiverService>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
            var result = await controller.Delete("bad_guid_but_unit_test_works_cause_validation_doesnt");

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteOneAsync("bad_guid_but_unit_test_works_cause_validation_doesnt"),
                Times.Once);
        }
    }
}