using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Retroactiune.Database;
using Retroactiune.Models;

namespace Retroactiune.IntegrationTests.Retroactiune.WebAPI.Fixtures
{
    public class MongoDbFixture : IAsyncDisposable
    {
        private readonly IDatabaseSettings _settings;
        private IMongoDatabase Database { get; }

        public IMongoCollection<FeedbackReceiver> FeedbackReceiverCollection =>
            Database.GetCollection<FeedbackReceiver>(_settings.FeedbackReceiverCollectionName);

        public IMongoCollection<Token> TokensCollection =>
            Database.GetCollection<Token>(_settings.TokensCollectionName);

        public MongoDbFixture(IOptions<DatabaseSettings> options)
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

        public async ValueTask DisposeAsync()
        {
            await DropAsync();
        }
    }
}