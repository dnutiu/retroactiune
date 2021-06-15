using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Retroactiune.Database;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Retroactiune.Models;
using Xunit;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    [Collection("IntegrationTests")]
    public class TestTokens : IClassFixture<WebApiTestingFactory>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestTokens(WebApiTestingFactory factory)
        {
            _client = factory.CreateClient();
            var dbSettings = factory.Services.GetService<IOptions<DatabaseSettings>>();
            _mongoDb = new MongoDbFixture(dbSettings);
        }

        [Fact]
        public async Task Test_GenerateTokens_Ok()
        {
            // Arrange
            await _mongoDb.DropAsync();
            await _mongoDb.FeedbackReceiverCollection.InsertOneAsync(new FeedbackReceiver
            {
                Id = "123456789012345678901234"
            });

            var expiryTime = DateTime.Today.AddDays(1);

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/Tokens/",
                new StringContent(
                    $"{{\"numberOfTokens\": 2, \"feedbackReceiverId\": \"123456789012345678901234\", \"expiryTime\": \"{expiryTime.ToUniversalTime():O}\" }}",
                    Encoding.UTF8,
                    "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            var tokens = (await _mongoDb.TokensCollection.FindAsync(FilterDefinition<Token>.Empty)).ToList();
            Assert.Equal(2, tokens.Count);
            foreach (var t in tokens)
            {
                Assert.Equal("123456789012345678901234", t.FeedbackReceiverId);
                Assert.NotNull(t.ExpiryTime);
            }
        }

        [Fact]
        public async Task Test_GenerateTokens_ExpiryInThePast()
        {
            // Arrange
            await _mongoDb.DropAsync();
            await _mongoDb.FeedbackReceiverCollection.InsertOneAsync(new FeedbackReceiver
            {
                Id = "123456789012345678901234"
            });

            var expiryTime = DateTime.Today.AddDays(-1);

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/Tokens/",
                new StringContent(
                    $"{{\"numberOfTokens\": 2, \"feedbackReceiverId\": \"123456789012345678901234\", \"expiryTime\": \"{expiryTime.ToUniversalTime():O}\" }}",
                    Encoding.UTF8,
                    "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }
        
        [Fact]
        public async Task Test_GenerateTokens_NonExistingFeedbackReceiver()
        {
            var httpResponse = await _client.PostAsync("/api/v1/Tokens/",
                new StringContent("{\"numberOfTokens\": 1, \"feedbackReceiverId\": \"someid\"}", Encoding.UTF8,
                    "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_GenerateTokens_NoBody()
        {
            var httpResponse = await _client.PostAsync("/api/v1/Tokens/",
                new StringContent("{}", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }
    }
}