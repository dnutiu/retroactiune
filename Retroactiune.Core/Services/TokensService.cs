﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;

namespace Retroactiune.Core.Services
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

        public async Task<IEnumerable<Token>> ListTokens(TokenListFilters filters)
        {
            // TODO Write unit tests.
            // TODO: Implement
            throw new NotImplementedException();
        }
    }
}