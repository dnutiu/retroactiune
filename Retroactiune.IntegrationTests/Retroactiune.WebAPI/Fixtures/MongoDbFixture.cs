using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Retroactiune.Models;
using Retroactiune.Settings;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures
{
    public class MongoDbFixture : IDisposable
    {
        private IMongoDbSettings _settings;
        public IMongoDatabase Database { get; }

        public IMongoCollection<FeedbackReceiver> FeedbackReceiverCollection =>
            Database.GetCollection<FeedbackReceiver>(_settings.FeedbackReceiverCollectionName);

        public MongoDbFixture(IOptions<RetroactiuneDbSettings> options)
        {
            _settings = options.Value;
            var client = new MongoClient(_settings.ConnectionString);
            Database = client.GetDatabase(_settings.DatabaseName);
        }

        public async Task DropAsync()
        {
            await Task.WhenAll(
                new List<Task>()
                {
                    Database.DropCollectionAsync(_settings.FeedbackCollectionName),
                    Database.DropCollectionAsync(_settings.FeedbackReceiverCollectionName),
                    Database.DropCollectionAsync(_settings.TokensCollectionName)
                });
        }


        public void Dispose()
        {
            DropAsync().GetAwaiter().GetResult();
        }
    }
}