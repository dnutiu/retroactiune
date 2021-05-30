using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public FeedbackReceiverService(IMongoClient client, IMongoDbSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<FeedbackReceiver>(settings.FeedbackReceiverCollectionName);
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