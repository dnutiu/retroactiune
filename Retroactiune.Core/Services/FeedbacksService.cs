using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MongoDB.Driver;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;

namespace Retroactiune.Core.Services
{
    public class FeedbacksService : IFeedbacksService
    {
        private readonly IMongoCollection<Feedback> _collection;

        public FeedbacksService(IMongoClient client, IDatabaseSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Feedback>(settings.FeedbacksCollectionName);
        }

        public async Task AddFeedbackAsync(Feedback feedback, FeedbackReceiver receiver)
        {
            Guard.Against.Null(feedback, nameof(feedback));
            Guard.Against.Null(receiver, nameof(receiver));

            feedback.FeedbackReceiverId = receiver.Id;
            await _collection.InsertOneAsync(feedback);
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksAsync(FeedbacksListFilters filters)
        {
            // TODO: Unit test.
            Guard.Against.Null(filters, nameof(filters));
            Guard.Against.Null(filters.FeedbackReceiverId, nameof(filters.FeedbackReceiverId));
            
            var filterBuilder = new FilterDefinitionBuilder<Feedback>();
            var activeFilters = new List<FilterDefinition<Feedback>>
            {
                // Filter tokens by their assigned feedback receiver id.
                filterBuilder.Eq(i => i.FeedbackReceiverId, filters.FeedbackReceiverId)
            };

            // Datetime after
            if (filters.CreatedAfter != null)
            {
                activeFilters.Add(filterBuilder.Gte(i => i.CreatedAt, filters.CreatedAfter));
            }

            // Datetime before
            if (filters.CreatedBefore != null)
            {
                activeFilters.Add(filterBuilder.Lte(i => i.CreatedAt, filters.CreatedBefore));
            }

            var results = await _collection.FindAsync(filterBuilder.And(activeFilters));
            return await results.ToListAsync();
        }
    }
}