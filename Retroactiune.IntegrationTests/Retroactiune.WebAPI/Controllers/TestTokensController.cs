using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Retroactiune.Core.Entities;
using Retroactiune.Infrastructure;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    [Collection("IntegrationTests")]
    public class TestTokensController : IClassFixture<WebApiTestingFactory>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestTokensController(WebApiTestingFactory factory)
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
            foreach (var i in tokensArr)
            {
                i.Id = ObjectId.GenerateNewId().ToString();
                i.FeedbackReceiverId = ObjectId.GenerateNewId().ToString();
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
            foreach (var i in tokensArr)
            {
                i.Id = ObjectId.GenerateNewId().ToString();
                i.FeedbackReceiverId = ObjectId.GenerateNewId().ToString();
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

        [Fact]
        public async Task Test_ListTokens_NoFilter_Empty()
        {
            // Setup
            await _mongoDb.DropAsync();

            // Test
            var response = await _client.GetAsync("api/v1/Tokens");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(items);
        }

        [Fact]
        public async Task Test_ListTokens_NoFilter()
        {
            // Setup
            await _mongoDb.DropAsync();
            var timeNow = DateTime.UtcNow;
            var tokens = TokensFixture.Generate(10, timeNow);
            await _mongoDb.TokensCollection.InsertManyAsync(tokens);

            // Test
            var response = await _client.GetAsync("api/v1/Tokens");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(tokens.Count, items.Count);
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(tokens[i], items[i]);
            }
        }

        [Fact]
        public async Task Test_ListTokens_Filter_FeedbackReceiverId()
        {
            // Setup
            await _mongoDb.DropAsync();

            var timeNow = DateTime.UtcNow;
            var tokens = TokensFixture.Generate(13, timeNow);
            var expectedTokens = TokensFixture.Generate(1, timeNow);

            var qb = new QueryBuilder
            {
                {"FeedbackReceiverId", expectedTokens[0].FeedbackReceiverId},
            };

            await _mongoDb.TokensCollection.InsertManyAsync(tokens);
            await _mongoDb.TokensCollection.InsertManyAsync(expectedTokens);

            // Test
            var response = await _client.GetAsync($"api/v1/Tokens{qb}");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(items);
            Assert.Equal(expectedTokens[0], items[0]);
        }

        [Fact]
        public async Task Test_ListTokens_Filter_Ids()
        {
            // Setup
            await _mongoDb.DropAsync();

            var timeNow = DateTime.UtcNow;
            var tokens = TokensFixture.Generate(13, timeNow);
            var expectedTokens = TokensFixture.Generate(1, timeNow);

            var qb = new QueryBuilder
            {
                {"Ids", expectedTokens[0].Id},
            };

            await _mongoDb.TokensCollection.InsertManyAsync(tokens);
            await _mongoDb.TokensCollection.InsertManyAsync(expectedTokens);

            // Test
            var response = await _client.GetAsync($"api/v1/Tokens{qb}");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Single(items);
            Assert.Equal(expectedTokens[0], items[0]);
        }

        [Fact]
        public async Task Test_ListTokens_Filter_CreatedRange()
        {
            // Setup
            await _mongoDb.DropAsync();

            var timeNow = DateTime.UtcNow;

            var oldTokens = TokensFixture.Generate(13, timeNow.AddDays(-10));
            var futureTokens = TokensFixture.Generate(13, timeNow.AddDays(10));
            var expectedTokens = TokensFixture.Generate(5, timeNow);

            var qb = new QueryBuilder
            {
                {"CreatedAfter", timeNow.AddDays(-3).ToString(CultureInfo.InvariantCulture)},
                {"CreatedBefore", timeNow.AddDays(3).ToString(CultureInfo.InvariantCulture)},
            };

            await _mongoDb.TokensCollection.InsertManyAsync(oldTokens);
            await _mongoDb.TokensCollection.InsertManyAsync(expectedTokens);
            await _mongoDb.TokensCollection.InsertManyAsync(futureTokens);

            // Test
            var response = await _client.GetAsync($"api/v1/Tokens{qb}");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedTokens.Count, items.Count);
            for (var i = 0; i < items.Count; i++)
            {
                Assert.Equal(expectedTokens[i], items[i]);
            }
        }

        [Fact]
        public async Task Test_ListTokens_Filter_UsedRange()
        {
            // Setup
            await _mongoDb.DropAsync();

            var timeNow = DateTime.UtcNow;

            var oldTokens = TokensFixture.Generate(13, timeNow.AddDays(-10));
            var futureTokens = TokensFixture.Generate(13, timeNow.AddDays(10));
            var expectedTokens = TokensFixture.Generate(5, timeNow, null, null, timeNow);

            var qb = new QueryBuilder
            {
                {"UsedAfter", timeNow.AddDays(-3).ToString(CultureInfo.InvariantCulture)},
                {"UsedBefore", timeNow.AddDays(3).ToString(CultureInfo.InvariantCulture)},
            };

            await _mongoDb.TokensCollection.InsertManyAsync(oldTokens);
            await _mongoDb.TokensCollection.InsertManyAsync(expectedTokens);
            await _mongoDb.TokensCollection.InsertManyAsync(futureTokens);

            // Test
            var response = await _client.GetAsync($"api/v1/Tokens{qb}");
            var items = JsonSerializer.Deserialize<List<Token>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(expectedTokens.Count, items.Count);
            for (var i = 0; i < items.Count; i++)
            {
                Assert.Equal(expectedTokens[i], items[i]);
            }
        }
    }
}