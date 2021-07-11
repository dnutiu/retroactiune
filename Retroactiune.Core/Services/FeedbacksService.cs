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
    }
}