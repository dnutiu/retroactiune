using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Moq;
using Retroactiune.Models;
using Retroactiune.Services;
using Retroactiune.Settings;
using Xunit;

namespace Retroactiune.Tests.Retroactiune.WebAPI.Services
{
    public class TestTokensService
    {
        [Fact]
        public async Task Test_GenerateTokensAsync_InvalidNumberOfTokens()
        {
            // Setup
            var mongoDatabaseMock = new Mock<IMongoDatabase>();
            var mongoClientMock = new Mock<IMongoClient>();
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
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
            var mongoSettingsMock = new Mock<IMongoDbSettings>();
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
            var item = new Token
            {
                Id = null,
                ExpiryTime = null,
                TimeUsed = null,
                FeedbackReceiverId = "Hello",
                CreatedAt = expiryTime
            };
            mongoCollectionMock.Verify(
                i => i.InsertManyAsync(new[] {item, item, item},
                    It.IsAny<InsertManyOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}