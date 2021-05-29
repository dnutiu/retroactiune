using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Retroactiune.Models;
using Retroactiune.Settings;

namespace Retroactiune.Services
{
    /// <summary>
    /// Service that simplifies access to the database for managing FeedbackReceiver items.
    /// <see cref="FeedbackReceiver"/>
    /// </summary>
    public class FeedbackReceiverService : IFeedbackReceiverService
    {
        private readonly IMongoCollection<FeedbackReceiver> _collection;
        private readonly ILogger<FeedbackReceiverService> _logger;

        public FeedbackReceiverService(IMongoDbSettings settings, ILogger<FeedbackReceiverService> logger)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<FeedbackReceiver>(settings.FeedbackReceiverCollectionName);
            _logger = logger;
        }

        public async Task CreateManyAsync(IEnumerable<FeedbackReceiver> items)
        {
            try
            {
                await _collection.InsertManyAsync(items);
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message}");
            }
            
        }
    }
}