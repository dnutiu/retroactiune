using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

        public MongoDbFixture()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Testing.json").Build();

            var section = config.GetSection(nameof(RetroactiuneDbSettings));

            _settings = new RetroactiuneDbSettings();
            _settings.ConnectionString = section.GetValue<string>("ConnectionString");
            _settings.TokensCollectionName = section.GetValue<string>("TokensCollectionName");
            _settings.FeedbackCollectionName = section.GetValue<string>("FeedbackCollectionName");
            _settings.FeedbackReceiverCollectionName = section.GetValue<string>("FeedbackReceiverCollectionName");
            _settings.DatabaseName = section.GetValue<string>("DatabaseName");

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