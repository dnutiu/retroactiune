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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Retroactiune.Models;
using Retroactiune.Settings;
using Xunit;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    public class TestFeedbackReceiver : IClassFixture<WebApiTestingFactory>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestFeedbackReceiver(WebApiTestingFactory factory)
        {
            _client = factory.CreateClient();
            var dbSettings = factory.Services.GetService<IOptions<RetroactiuneDbSettings>>();
            _mongoDb = new MongoDbFixture(dbSettings);
        }


        [Fact]
        public async Task Test_Create_NoContent()
        {
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceivers/",
                new StringContent("[]", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_Create_NoName()
        {
            // Arrange
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
            var guids = new List<string>();
            await _mongoDb.DropAsync();
            byte index = 0;
            var cleanItems = items.Select(i =>
            {
                i.Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, index, 4, 5, 6, 7, 8, 9, index, 11, 14}))
                    .ToString();
                index += 1;
                guids.Add(i.Id);
                return i;
            });
            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(cleanItems);


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
            var feedbackReceiver = new FeedbackReceiver()
            {
                Id = new BsonObjectId(new ObjectId(new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 14})).ToString(),
                Name = "N4m3",
                Description = "someting",
                CreatedAt = DateTime.Parse("2020-02-01")
            };

            await _mongoDb.FeedbackReceiverCollection.InsertManyAsync(new[] {feedbackReceiver});

            // Test
            var httpResponse =
                await _client.GetAsync($"/api/v1/FeedbackReceivers/{feedbackReceiver.Id}", CancellationToken.None);
            var item = await httpResponse.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.Equal(
                "{\"id\":\"0102030405060708090a0b0e\",\"name\":\"N4m3\",\"description\":\"someting\",\"createdAt\":\"2020-01-31T22:00:00Z\"}",
                item);
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
        
        // TODO test list
    }
}