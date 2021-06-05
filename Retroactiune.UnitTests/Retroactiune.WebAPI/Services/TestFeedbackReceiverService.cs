﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using MongoDB.Driver;
using Moq;
using Retroactiune.Models;
using Retroactiune.Services;
using Retroactiune.Settings;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.WebAPI.Services
{
    public class TestFeedbackReceiverService
    {
        [Theory, AutoData]
        public async Task Test_CreateManyAsync_Success(IEnumerable<FeedbackReceiver> items)
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiverCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiverService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.CreateManyAsync(items);

            // Assert
            mongoClientMock.Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock.Verify(
                i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                Times.Once());
            mongoCollectionMock.Verify(
                i => i.InsertManyAsync(items, It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public void Test_CreateManyAsync_NullVal()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiverCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(i => i.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiverService(mongoClientMock.Object, mongoSettingsMock.Object);
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
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiverCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(i => i.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiverService(mongoClientMock.Object, mongoSettingsMock.Object);
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
        public async Task Test_Delete_Ok()
        {
            // Arrange
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<FeedbackReceiver>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.FeedbackReceiverCollectionName).Returns("feedback_receiver");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<FeedbackReceiver>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new FeedbackReceiverService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.DeleteOneAsync("insert_guid_here");

            // Assert
            mongoClientMock.Verify(i => i.GetDatabase("MyDB", null), Times.Once());
            mongoDatabaseMock.Verify(
                i => i.GetCollection<FeedbackReceiver>("feedback_receiver", It.IsAny<MongoCollectionSettings>()),
                Times.Once());

            mongoCollectionMock.Verify(
                i => i.DeleteOneAsync(It.IsAny<FilterDefinition<FeedbackReceiver>>(), It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}