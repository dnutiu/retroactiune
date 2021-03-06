using System.Collections.Generic;
using System.Linq;
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
    public class TestFeedbackReceiverService
    {
        [Theory, AutoData]
        public async Task Test_CreateManyAsync_Success(IEnumerable<FeedbackReceiver> items)
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiversCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiversService(mongoClientMock.Object, mongoSettingsMock.Object);
            var feedbackReceivers = items as FeedbackReceiver[] ?? items.ToArray();
            await service.CreateManyAsync(feedbackReceivers);

            // Assert
            mongoClientMock.Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock.Verify(
                i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                Times.Once());
            mongoCollectionMock.Verify(
                i => i.InsertManyAsync(feedbackReceivers, It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public void Test_CreateManyAsync_NullVal()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiversCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(i => i.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiversService(mongoClientMock.Object, mongoSettingsMock.Object);
            var ex = Record.ExceptionAsync(async () => await service.CreateManyAsync(null));


            // Assert
            Assert.NotNull(ex);

            mongoClientMock
                .Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock
                .Verify(
                    i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                    Times.Once());
            mongoCollectionMock
                .Verify(
                    i => i.InsertManyAsync(It.IsAny<IEnumerable<FeedbackReceiver>>(), It.IsAny<InsertManyOptions>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never());
        }

        [Fact]
        public void Test_CreateManyAsync_NoItems()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiversCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(i => i.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiversService(mongoClientMock.Object, mongoSettingsMock.Object);
            var ex = Record.ExceptionAsync(async () => await service.CreateManyAsync(new List<FeedbackReceiver>()));


            // Assert
            Assert.NotNull(ex);

            mongoClientMock
                .Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock
                .Verify(
                    i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                    Times.Once());
            mongoCollectionMock
                .Verify(
                    i => i.InsertManyAsync(It.IsAny<IEnumerable<FeedbackReceiver>>(), It.IsAny<InsertManyOptions>(),
                        It.IsAny<CancellationToken>()),
                    Times.Never());
        }

        [Fact]
        public async Task Test_DeleteManyAsync_Ok()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiversCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiversService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.DeleteManyAsync(new []{"insert_guid_here"});

            // Assert
            mongoClientMock.Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock.Verify(
                i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                Times.Once());

            mongoCollectionMock.Verify(
                i => i.DeleteManyAsync(It.IsAny<FilterDefinition<FeedbackReceiver>>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task Test_FindAsync_Ok()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();
            var mongoCursorMock = new Mock<IAsyncCursor<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiversCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            mongoCollectionMock
                .Setup(i => i.FindAsync(It.IsAny<FilterDefinition<FeedbackReceiver>>(),
                    It.IsAny<FindOptions<FeedbackReceiver, FeedbackReceiver>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mongoCursorMock.Object);

            // Test
            var service = new FeedbackReceiversService(mongoClientMock.Object, mongoSettingsMock.Object);
            var guids = new[] {"insert_guid_here"};
            await service.FindAsync(guids, 0, 15);

            // Assert
            mongoClientMock.Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock.Verify(
                i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                Times.Once());
            mongoCollectionMock.Verify(
                i => i.FindAsync(It.IsAny<FilterDefinition<FeedbackReceiver>>(),
                    It.IsAny<FindOptions<FeedbackReceiver, FeedbackReceiver>>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}