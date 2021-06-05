using System;
using System.Collections.Generic;
using System.Linq;
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
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var feedbackReceivers = items as FeedbackReceiver[] ?? items.ToArray();
            if (!feedbackReceivers.Any())
            {
                throw new GenericServiceException("items must contain at least one element");
            }

            try
            {
                await _collection.InsertManyAsync(feedbackReceivers);
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message} {e.StackTrace}");
            }
        }

        public async Task DeleteOneAsync(string guid)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<FeedbackReceiver>();
                await _collection.DeleteOneAsync(filter.Eq(i => i.Id, guid));
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message} {e.StackTrace}");
            }
        }

        public async Task<IEnumerable<FeedbackReceiver>> FindAsync(IEnumerable<string> guids, int? offset = null,
            int? limit = null)
        {
            try
            {
                var guidsArr = guids as string[] ?? guids.ToArray();

                var filterBuilder = new FilterDefinitionBuilder<FeedbackReceiver>();
                var filterOptions = new FindOptions<FeedbackReceiver, FeedbackReceiver>();
                var filter = filterBuilder.Empty;

                // Filter for guids
                if (guidsArr.Any())
                {
                    filter = filterBuilder.In(i => i.Id, guidsArr);
                }

                // Set skip
                if (offset != null)
                {
                    filterOptions.Skip = offset;
                }

                // Set limit
                if (limit != null)
                {
                    filterOptions.Limit = limit;
                }

                var cursor = await _collection.FindAsync(filter, filterOptions);
                return cursor.ToList();
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message} {e.StackTrace}");
            }
        }
    }
}