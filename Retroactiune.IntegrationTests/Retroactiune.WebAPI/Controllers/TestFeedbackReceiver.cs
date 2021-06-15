using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Retroactiune.Database;
using Retroactiune.DataTransferObjects;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Retroactiune.Models;
using Xunit;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    [Collection("IntegrationTests")]
    public class TestFeedbackReceiver : IClassFixture<WebApiTestingFactory>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestFeedbackReceiver(WebApiTestingFactory factory)
        {
            _client = factory.CreateClient();
            var dbSettings = factory.Services.GetService<IOptions<DatabaseSettings>>();
            _mongoDb = new MongoDbFixture(dbSettings);
        }


        [Fact]
        public async Task Test_Create_NoContent()
        {
            await _mongoDb.DropAsync();
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceivers/",
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
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceivers/",
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
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceivers/",
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
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceivers/",
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
                await _client.DeleteAsync("/api/v1/FeedbackReceivers/abc_not_guid", CancellationToken.None);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Theory, AutoData]
        public async Task Test_Delete_OK(IEnumerable<FeedbackReceiver> items)
        {
            // Arrange
            await _mongoDb.DropAsync();
            var guids = new List<string>();
            await _mongoDb.DropAsync();
            byte index = 0;
            var feedbackReceivers = items as FeedbackReceiver[] ?? items.ToArray();
            foreach (var i in feedbackReceivers)
            {
                i.Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, index, 4, 5, 6, 7, 8, 9, index, 11, 14}))
                    .ToString();
                index += 1;
                guids.Add(i.Id);
            }
            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            foreach (var guid in guids)
            {
                var httpResponse =
                    await _client.DeleteAsync($"/api/v1/FeedbackReceivers/{guid}", CancellationToken.None);
                Assert.Equal(HttpStatusCode.NoContent, httpResponse.StatusCode);
            }

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
                Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                Name = "N4m3",
                Description = "something",
                CreatedAt = DateTime.UnixEpoch
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(new[] {feedbackReceiver});

            // Test
            var httpResponse =
                await _client.GetAsync($"/api/v1/FeedbackReceivers/{feedbackReceiver.Id}", CancellationToken.None);
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
            var httpResponse = await _client.GetAsync("/api/v1/FeedbackReceivers/0102030405060708090a0b0e",
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
            var httpResponse = await _client.GetAsync("/api/v1/FeedbackReceivers/xxx", CancellationToken.None);

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
                    Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {3, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var httpResponse = await _client.GetAsync("/api/v1/FeedbackReceivers/", CancellationToken.None);
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
                    Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {3, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var qb = new QueryBuilder {{"filter", new[] {feedbackReceivers[0].Id, feedbackReceivers[1].Id}}};
            var httpResponse =
                await _client.GetAsync("/api/v1/FeedbackReceivers/" + qb,
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
                    Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {2, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Two",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                },
                new FeedbackReceiver
                {
                    Id = new BsonObjectId(new ObjectId(new byte[] {3, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                    Name = "N4m3_Three",
                    Description = "something",
                    CreatedAt = DateTime.UnixEpoch
                }
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(feedbackReceivers);


            // Test
            var qb = new QueryBuilder {{"offset", "1"}, {"limit", "1"}};
            var httpResponse = await _client.GetAsync("/api/v1/FeedbackReceivers/" + qb, CancellationToken.None);
            var items = JsonSerializer.Deserialize<List<FeedbackReceiver>>(
                await httpResponse.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Single(items);
            Assert.Equal(feedbackReceivers[1], items[0]);
        }
    }
}