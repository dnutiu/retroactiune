using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retroactiune.Database;
using Retroactiune.Models;

namespace Retroactiune.Services
{
    public class TokensService : ITokensService
    {
        private readonly IMongoCollection<Token> _collection;

        public TokensService(IMongoClient client, IDatabaseSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Token>(settings.TokensCollectionName);
        }

        public async Task GenerateTokensAsync(int numberOfTokens, string feedbackReceiverGuid,
            DateTime? expiryTime = null)
        {
            if (numberOfTokens <= 0)
            {
                throw new ArgumentException("numberOfTokens must be positive");
            }

            var token = new List<Token>();
            for (var i = 0; i < numberOfTokens; i++)
            {
                token.Add(new Token
                {
                    CreatedAt = DateTime.UtcNow,
                    FeedbackReceiverId = feedbackReceiverGuid,
                    ExpiryTime = expiryTime,
                    TimeUsed = null
                });
            }

            await _collection.InsertManyAsync(token);
        }

        public async Task DeleteTokens(IEnumerable<string> tokenIds)
        {
            // TODO: Unit test.
            try
            {
                var filter = new FilterDefinitionBuilder<Token>();
                await _collection.DeleteManyAsync(filter.In(i => i.Id, tokenIds));
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message} {e.StackTrace}");
            }
        }
    }
}