using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Retroactiune.Core.Entities;
using Retroactiune.Infrastructure;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
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
                new StringContent("{\"numberOfTokens\": 1, \"feedbackReceiverId\": \"some_id\"}", Encoding.UTF8,
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

        // Delete nok, DeleteMany nok

        [Theory, AutoData]
        public async Task Test_Delete_Ok(IEnumerable<Token> tokens)
        {
            // Setup
            await _mongoDb.DropAsync();
            var guids = new List<string>();
            var tokensArr = tokens as Token[] ?? tokens.ToArray();
            byte index = 0;
            foreach (var i in tokensArr)
            {
                i.Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, index, 4, 5, 6, 7, 8, 9, index, 11, 14}))
                    .ToString();
                i.FeedbackReceiverId = i.Id;
                index += 1;
                guids.Add(i.Id);
            }

            await _mongoDb.TokensCollection.InsertManyAsync(tokensArr);
            Assert.Equal(tokensArr.Length,
                await _mongoDb.TokensCollection.CountDocumentsAsync(FilterDefinition<Token>.Empty));

            // Test
            foreach (var guid in guids)
            {
                var response = await _client.DeleteAsync($"/api/v1/Tokens/{guid}", CancellationToken.None);
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }

            // Assert
            Assert.Equal(0, await _mongoDb.TokensCollection.CountDocumentsAsync(FilterDefinition<Token>.Empty));
        }

        [Fact]
        public async Task Test_Delete_BadGuid()
        {
            // Setup
            await _mongoDb.DropAsync();

            // Test
            var response = await _client.DeleteAsync("/api/v1/Tokens/batman", CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, AutoData]
        public async Task Test_DeleteMany_Ok(IEnumerable<Token> tokens)
        {
            // Setup
            await _mongoDb.DropAsync();
            var guids = new List<string>();
            var tokensArr = tokens as Token[] ?? tokens.ToArray();
            byte index = 0;
            foreach (var i in tokensArr)
            {
                i.Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, index, 4, 5, 6, 7, 8, 9, index, 11, 14}))
                    .ToString();
                i.FeedbackReceiverId = i.Id;
                index += 1;
                guids.Add(i.Id);
            }

            await _mongoDb.TokensCollection.InsertManyAsync(tokensArr);
            Assert.Equal(tokensArr.Length,
                await _mongoDb.TokensCollection.CountDocumentsAsync(FilterDefinition<Token>.Empty));

            // Test
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_client.BaseAddress.AbsoluteUri}api/v1/Tokens"),
                // ReSharper disable once MethodHasAsyncOverload
                Content = new StringContent(JsonConvert.SerializeObject(guids), Encoding.UTF8, "application/json")
            };
            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Assert
            Assert.Equal(0, await _mongoDb.TokensCollection.CountDocumentsAsync(FilterDefinition<Token>.Empty));
        }

        [Fact]
        public async Task Test_DeleteMany_BadRequest()
        {
            // Setup
            await _mongoDb.DropAsync();

            // Test
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_client.BaseAddress.AbsoluteUri}api/v1/Tokens"),
                // ReSharper disable once MethodHasAsyncOverload
                Content = new StringContent(JsonConvert.SerializeObject(new[] {"bad", "badder"}), Encoding.UTF8,
                    "application/json")
            };
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}