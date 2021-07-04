using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.Core.Services
{
    public class TestTokensService
    {
        [Fact]
        public async Task Test_GenerateTokensAsync_InvalidNumberOfTokens()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test & Assert
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            await Assert.ThrowsAsync<ArgumentException>(async () => { await service.GenerateTokensAsync(-1, ""); });
        }

        [Fact]
        public async Task Test_GenerateTokensAsync_Success()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var expiryTime = DateTime.UtcNow;
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.GenerateTokensAsync(3, "Hello", expiryTime);

            // Assert
            mongoCollectionMock.Verify(
                i => i.InsertManyAsync(It.IsAny<IEnumerable<Token>>(),
                    It.IsAny<InsertManyOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteTokens_Ok()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.DeleteTokensAsync(new[] {"test_id"});

            // Assert
            mongoCollectionMock.Verify(
                i
                    => i.DeleteManyAsync(
                        It.IsAny<FilterDefinition<Token>>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Test_ListTokens_NoFilters_Ok()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();
            var mongoCursorMock = new Mock<IAsyncCursor<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            mongoCollectionMock.Setup(i => i.FindAsync(It.IsAny<FilterDefinition<Token>>(),
                    It.IsAny<FindOptions<Token, Token>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mongoCursorMock.Object);

            // Test
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            var result = await service.FindAsync(new TokenListFilters());

            // Assert
            Assert.IsType<List<Token>>(result);
            mongoCollectionMock.Verify(
                i
                    => i.FindAsync(It.IsAny<FilterDefinition<Token>>(),
                        It.IsAny<FindOptions<Token, Token>>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Test_ListTokens_Filters_Ok()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();
            var mongoCursorMock = new Mock<IAsyncCursor<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            mongoCollectionMock.Setup(i => i.FindAsync(It.IsAny<FilterDefinition<Token>>(),
                    It.IsAny<FindOptions<Token, Token>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mongoCursorMock.Object);

            // Test
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            var result = await service.FindAsync(new TokenListFilters
            {
                Ids = new[] {"a", "b"},
                FeedbackReceiverId = "abc",
                CreatedAfter = DateTime.UtcNow,
                CreatedBefore = DateTime.UtcNow,
                UsedAfter = DateTime.UtcNow,
                UsedBefore = DateTime.UtcNow
            });

            // Assert
            Assert.IsType<List<Token>>(result);
            mongoCollectionMock.Verify(
                i
                    => i.FindAsync(It.IsAny<FilterDefinition<Token>>(),
                        It.IsAny<FindOptions<Token, Token>>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteManyByFeedbackReceiverIdAsync_Ok()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            // Test
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            await service.DeleteManyByFeedbackReceiverIdAsync(new[] {"test_id"});

            // Assert
            mongoCollectionMock.Verify(
                i
                    => i.DeleteManyAsync(
                        It.IsAny<FilterDefinition<Token>>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Test_DeleteManyByFeedbackReceiverIdAsync_Exception()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IDatabaseSettings>();
            var mongoCollectionMock = new Mock<IMongoCollection<Token>>();

            mongoSettingsMock.SetupGet(i => i.DatabaseName).Returns("MyDB");
            mongoSettingsMock.SetupGet(i => i.TokensCollectionName).Returns("tokens");

            mongoClientMock
                .Setup(stub => stub.GetDatabase(It.IsAny<string>(),
                    It.IsAny<MongoDatabaseSettings>()))
                .Returns(mongoDatabaseMock.Object);

            mongoDatabaseMock
                .Setup(i => i.GetCollection<Token>(It.IsAny<string>(),
                    It.IsAny<MongoCollectionSettings>()))
                .Returns(mongoCollectionMock.Object);

            mongoCollectionMock.Setup(i =>
                    i.DeleteManyAsync(It.IsAny<FilterDefinition<Token>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new GenericServiceException("op failed"));
            // Test
            var service = new TokensService(mongoClientMock.Object, mongoSettingsMock.Object);
            await Assert.ThrowsAsync<GenericServiceException>(async () =>
            {
                await service.DeleteManyByFeedbackReceiverIdAsync(new[] {"test_id"});
            });

            // Assert
            mongoCollectionMock.Verify(
                i
                    => i.DeleteManyAsync(
                        It.IsAny<FilterDefinition<Token>>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}