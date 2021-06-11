using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retroactiune.Models;
using Retroactiune.Settings;

namespace Retroactiune.Services
{
    public class TokensService : ITokensService
    {
        private readonly IMongoCollection<Token> _collection;

        public TokensService(IMongoClient client, IMongoDbSettings settings)
        {
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<Token>(settings.TokensCollectionName);
        }

        public async Task GenerateTokensAsync(int numberOfTokens, string feedbackReceiverGuid,
            DateTime? expiryTime = null)
        {
            // TODO: Test unit
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
                    ExpiryTime = null,
                    TimeUsed = null
                });
            }

            await _collection.InsertManyAsync(token);
        }
    }
}