using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using MongoDB.Driver;
using Moq;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.Core.Services
{
    public class TestFeedbacksService
    {
        [Fact]
        public async Task Test_AddFeedbackAsync_NullGuards()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Feedback>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbacksCollectionName).Returns("feedbacks");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Feedback>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test & Assert
            var service = new FeedbacksService(mongoClientMock.Object, mongoSettingsMock.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.AddFeedbackAsync(null, new FeedbackReceiver());
            });
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.AddFeedbackAsync(new Feedback(), null);
            });
        }

        [Theory, AutoData]
        public async Task Test_AddFeedbackAsync_Ok(Feedback feedback, FeedbackReceiver feedbackReceiver)
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Feedback>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbacksCollectionName).Returns("feedbacks");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Feedback>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbacksService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.AddFeedbackAsync(feedback, feedbackReceiver);

            // Assert
            feedback.FeedbackReceiverId = feedbackReceiver.Id;
            mongoCollectionMock.Verify(
                i => i.InsertOneAsync(
                    feedback,
                    It.IsAny<InsertOneOptions>(),
                    It.IsAny<CancellationToken>()));
        }
    }
}