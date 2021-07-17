using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Retroactiune.Core.Entities;
using Retroactiune.DataTransferObjects;
using Retroactiune.Infrastructure;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    [Collection("IntegrationTests")]
    public class TestFeedbackReceiversController : IClassFixture<WebApiTestingFactory>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestFeedbackReceiversController(WebApiTestingFactory factory)
        {
            _client = factory.CreateClient();
            var dbSettings = factory.Services.GetService<IOptions<DatabaseSettings>>();
            _mongoDb = new MongoDbFixture(dbSettings);
        }


        [Fact]
        public async Task Test_Create_NoContent()
        {
            await _mongoDb.DropAsync();
            var httpResponse = await _client.PostAsync("/api/v1/feedback_receivers/",
                new StringContent("[]", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Create_NoName()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var fixture = new Fixture();
            var item = fixture.Create<FeedbackReceiverInDto>();
            item.Name = null;

            var jsonContent = JsonSerializer.Serialize(new List<FeedbackReceiverInDto> {item});

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/feedback_receivers/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Create_NoDescription()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var fixture = new Fixture();
            var item = fixture.Create<FeedbackReceiverInDto>();
            item.Description = null;

            var jsonContent = JsonSerializer.Serialize(new List<FeedbackReceiverInDto> {item});

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/feedback_receivers/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Theory, AutoData]
        public async Task Test_Create_Ok(IEnumerable<FeedbackReceiverInDto> items)
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceiversDto = items.ToList();
            var jsonContent = JsonSerializer.Serialize(feedbackReceiversDto);

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/feedback_receivers/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            httpResponse.EnsureSuccessStatusCode();
            var filter = new FilterDefinitionBuilder<FeedbackReceiver>().Empty;

            var createdDocs = await _mongoDb.FeedbackReceiverCollection.CountDocumentsAsync(filter);
            Assert.Equal(feedbackReceiversDto.Count(), createdDocs);
        }

        [Fact]
        public async Task Test_Delete_ValidationFail()
        {
            // Arrange
            await _mongoDb.DropAsync();

            // Test
            var httpResponse =
                await _client.DeleteAsync("/api/v1/feedback_receivers/abc_not_guid", CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Theory, AutoData]
        public async Task Test_Delete_OK(IEnumerable<FeedbackReceiver> items)
        {
            // Arrange
            await _mongoDb.DropAsync();
            var guids = new List<string>();
            var feedbackReceivers = items as FeedbackReceiver[] ?? items.ToArray();
            foreach (var i in feedbackReceivers)
            {
                i.Id = ObjectId.GenerateNewId().ToString();
                guids.Add(i.Id);
            }

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            foreach (var guid in guids)
            {
                var httpResponse =
                    await _client.DeleteAsync($"/api/v1/feedback_receivers/{guid}", CancellationToken.None);
                Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
            }

            // Assert
            var docs = await _mongoDb.FeedbackReceiverCollection.CountDocumentsAsync(FilterDefinition<FeedbackReceiver>
                .Empty);
            Assert.Equal(0L, docs);
        }

        [Fact]
        public async Task Test_Delete_OK_With_Tokens()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceiver = new FeedbackReceiver
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = "blame",
                CreatedAt = DateTime.UtcNow,
                Name = "test"
            };
            await _mongoDb.FeedbackReceiverCollection.InsertOneAsync(feedbackReceiver);


            // Test
            var httpResponse = await _client.DeleteAsync($"/api/v1/feedback_receivers/{feedbackReceiver.Id}",
                CancellationToken.None);
            await _client.PostAsync("/api/v1/Tokens/",
                new StringContent($"{{\"numberOfTokens\": 100, \"feedbackReceiverId\": \"{feedbackReceiver.Id}\"}}",
                    Encoding.UTF8,
                    "application/json"));


            // Assert
            Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
            var feedbackReceivers =
                await _mongoDb.FeedbackReceiverCollection.CountDocumentsAsync(FilterDefinition<FeedbackReceiver>.Empty);
            var tokens = await _mongoDb.TokensCollection.CountDocumentsAsync(FilterDefinition<Token>.Empty);
            Assert.Equal(0L, feedbackReceivers);
            Assert.Equal(0L, tokens);
        }

        [Theory, AutoData]
        public async Task Test_DeleteMany_OK(IEnumerable<FeedbackReceiver> items)
        {
            // Arrange
            await _mongoDb.DropAsync();
            var guids = new List<string>();
            var feedbackReceivers = items as FeedbackReceiver[] ?? items.ToArray();
            foreach (var i in feedbackReceivers)
            {
                i.Id = ObjectId.GenerateNewId().ToString();
                guids.Add(i.Id);
            }

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{_client.BaseAddress.AbsoluteUri}api/v1/feedback_receivers"),
                // ReSharper disable once MethodHasAsyncOverload
                Content = new StringContent(JsonConvert.SerializeObject(guids), Encoding.UTF8, "application/json")
            };
            var httpResponse = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);

            // Assert
            var docs = await _mongoDb.FeedbackReceiverCollection.CountDocumentsAsync(FilterDefinition<FeedbackReceiver>
                .Empty);
            Assert.Equal(0L, docs);
        }

        [Fact]
        public async Task Test_Get_Ok()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceiver = new FeedbackReceiver
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "N4m3",
                Description = "something",
                CreatedAt = DateTime.UnixEpoch
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(new[] {feedbackReceiver});

            // Test
            var httpResponse =
                await _client.GetAsync($"/api/v1/feedback_receivers/{feedbackReceiver.Id}", CancellationToken.None);
            var item = JsonSerializer.Deserialize<FeedbackReceiver>(
                await httpResponse.Content.ReadAsStringAsync());

            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(feedbackReceiver, item);
        }

        [Fact]
        public async Task Test_Get_NotFound()
        {
            // Arrange
            await _mongoDb.DropAsync();

            // Test
            var httpResponse = await _client.GetAsync("/api/v1/feedback_receivers/0102030405060708090a0b0e",
                CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Get_BadRequest()
        {
            // Arrange
            await _mongoDb.DropAsync();

            // Test
            var httpResponse = await _client.GetAsync("/api/v1/feedback_receivers/xxx", CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_List_Ok()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceivers = new List<FeedbackReceiver>
            {
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var httpResponse = await _client.GetAsync("/api/v1/feedback_receivers/", CancellationToken.None);
            var items = JsonSerializer.Deserialize<List<FeedbackReceiver>>(
                await httpResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            for (var i = 0; i < feedbackReceivers.Count; i++)
            {
                Assert.Equal(feedbackReceivers[i], items[i]);
            }
        }

        [Fact]
        public async Task Test_List_Ok_Filter()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceivers = new List<FeedbackReceiver>
            {
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var qb = new QueryBuilder {{"filter", new[] {feedbackReceivers[0].Id, feedbackReceivers[1].Id}}};
            var httpResponse =
                await _client.GetAsync("/api/v1/feedback_receivers/" + qb,
                    CancellationToken.None);
            var items = JsonSerializer.Deserialize<List<FeedbackReceiver>>(
                await httpResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(2, items.Count);
            for (var i = 0; i < 2; i++)
            {
                Assert.Equal(feedbackReceivers[i], items[i]);
            }
        }

        [Fact]
        public async Task Test_List_Ok_LimitOffset()
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceivers = new List<FeedbackReceiver>
            {
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var qb = new QueryBuilder {{"offset", "1"}, {"limit", "1"}};
            var httpResponse = await _client.GetAsync("/api/v1/feedback_receivers/" + qb, CancellationToken.None);
            var items = JsonSerializer.Deserialize<List<FeedbackReceiver>>(
                await httpResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Single(items);
            Assert.Equal(feedbackReceivers[1], items[0]);
        }

        [Fact]
        public async Task Test_AddFeedback_Happy()
        {
            // Setup
            await _mongoDb.DropAsync();
            var feedbackReceiver = new FeedbackReceiver();
            var token = new Token
            {
                FeedbackReceiverId = feedbackReceiver.Id
            };
            await _mongoDb.FeedbackReceiverCollection.InsertOneAsync(feedbackReceiver);
            await _mongoDb.TokensCollection.InsertOneAsync(token);

            // Test
            var feedback = new FeedbackInDto
            {
                TokenId = token.Id,
                Description = "ok",
                Rating = 4
            };
            var content = JsonSerializer.Serialize(feedback);
            var response = await _client.PostAsync($"api/v1/feedback_receivers/feedbacks",
                new StringContent(content, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var feedbacksCursor = await _mongoDb.FeedbacksCollection.FindAsync(FilterDefinition<Feedback>.Empty);
            var feedbacks = await feedbacksCursor.ToListAsync();
            
            Assert.Equal("ok", feedbacks.ElementAt(0).Description);
            Assert.Equal(4u, feedbacks.ElementAt(0).Rating);
            Assert.Equal(feedbackReceiver.Id, feedbacks.ElementAt(0).FeedbackReceiverId);
            
            var tokensCursor = await _mongoDb.TokensCollection.FindAsync(FilterDefinition<Token>.Empty);
            var tokens = await tokensCursor.ToListAsync();
            
            Assert.NotNull(tokens.ElementAt(0).TimeUsed);
        }
    }
}