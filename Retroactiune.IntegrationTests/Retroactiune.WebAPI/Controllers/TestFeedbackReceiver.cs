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
using MongoDB.Driver;
using Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures;
using Retroactiune.Models;
using Xunit;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Controllers
{
    public class TestFeedbackReceiver : IClassFixture<WebApiTestingFactory>, IClassFixture<MongoDbFixture>
    {
        private readonly MongoDbFixture _mongoDb;
        private readonly HttpClient _client;

        public TestFeedbackReceiver(WebApiTestingFactory factory, MongoDbFixture mongoDbFixture)
        {
            _client = factory.CreateClient();
            _mongoDb = mongoDbFixture;
        }


        [Fact]
        public async Task Test_CreateFeedbackReceiver_NoContent()
        {
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceiver/",
                new StringContent("[]", Encoding.UTF8, "application/json"));
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_CreateFeedbackReceiver_NoName()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<FeedbackReceiverDto>();
            item.Name = null;

            var jsonContent = JsonSerializer.Serialize(new List<FeedbackReceiverDto> {item});

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceiver/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Fact]
        public async Task Test_CreateFeedbackReceiver_NoDescription()
        {
            // Arrange
            var fixture = new Fixture();
            var item = fixture.Create<FeedbackReceiverDto>();
            item.Description = null;

            var jsonContent = JsonSerializer.Serialize(new List<FeedbackReceiverDto> {item});

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceiver/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [Theory, AutoData]
        public async Task Test_CreateFeedbackReceiver_Ok(IEnumerable<FeedbackReceiverDto> items)
        {
            // Arrange
            await _mongoDb.DropAsync();
            var feedbackReceiversDto = items.ToList();
            var jsonContent = JsonSerializer.Serialize(feedbackReceiversDto);

            // Test
            var httpResponse = await _client.PostAsync("/api/v1/FeedbackReceiver/",
                new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            // Assert
            httpResponse.EnsureSuccessStatusCode();
            var filter = new FilterDefinitionBuilder<FeedbackReceiver>().Empty;

            var createdDocs = await _mongoDb.FeedbackReceiverCollection.CountDocumentsAsync(filter);
            Assert.Equal(feedbackReceiversDto.Count(), createdDocs);
        }
    }
}