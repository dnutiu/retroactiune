using System.Collections.Generic;
using System.Linq;
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
            mockService.Verify(s => s.DeleteManyAsync(new[] {"bad_guid_but_unit_test_works_cause_validation_doesnt"}),
                Times.Once);
        }

        [Fact]
        public async Task Get_Successful()
        {
            // Arrange
            var mapper = TestUtils.GetMapper();
            var mockService = new Mock<IFeedbackReceiverService>();
            mockService.Setup(i => i.FindAsync(It.IsAny<IEnumerable<string>>(), null, null))
                .ReturnsAsync(new[] {new FeedbackReceiver()});

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
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
            var mockService = new Mock<IFeedbackReceiverService>();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
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
            var mockService = new Mock<IFeedbackReceiverService>();
            var filterArr = filter as string[] ?? filter.ToArray();

            // Test
            var controller = new FeedbackReceiversController(mockService.Object, mapper, null);
            var result = await controller.List(filterArr, offset, limit);

            Assert.IsType<OkObjectResult>(result);
            mockService.Verify(s => s.FindAsync(filterArr, offset, limit), Times.Once);
        }
    }
}