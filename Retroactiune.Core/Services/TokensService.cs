using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
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
            Guard.Against.Negative(numberOfTokens, nameof(numberOfTokens));

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

        public async Task DeleteTokensAsync(IEnumerable<string> tokenIds)
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

        public async Task<IEnumerable<Token>> FindAsync(TokenListFilters filters)
        {
            var filterBuilder = new FilterDefinitionBuilder<Token>();
            var activeFilters = new List<FilterDefinition<Token>>();
            var tokensListFilter = FilterDefinition<Token>.Empty;

            // Filter by token ids.
            if (filters.Ids != null && filters.Ids.Any())
            {
                activeFilters.Add(filterBuilder.In(i => i.Id, filters.Ids));
            }

            // Filter tokens by their assigned feedback receiver id.
            if (filters.FeedbackReceiverId != null)
            {
                activeFilters.Add(filterBuilder.Eq(i => i.FeedbackReceiverId, filters.FeedbackReceiverId));
            }

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

            // Used time after
            if (filters.UsedAfter != null)
            {
                activeFilters.Add(filterBuilder.Gte(i => i.TimeUsed, filters.UsedAfter));
            }

            // Used time before
            if (filters.UsedBefore != null)
            {
                activeFilters.Add(filterBuilder.Lte(i => i.TimeUsed, filters.UsedBefore));
            }


            // Construct the final filter.
            if (activeFilters.Any())
            {
                tokensListFilter = filterBuilder.And(activeFilters);
            }

            var results = await _collection.FindAsync(tokensListFilter);
            return await results.ToListAsync();
        }

        public async Task DeleteManyByFeedbackReceiverIdAsync(IEnumerable<string> feedbackReceiverIds)
        {
            try
            {
                var filter = new FilterDefinitionBuilder<Token>();
                await _collection.DeleteManyAsync(filter.In(i => i.FeedbackReceiverId, feedbackReceiverIds));
            }
            catch (Exception e)
            {
                throw new GenericServiceException($"Operation failed: {e.Message} {e.StackTrace}");
            }
        }

        public async Task MarkTokenAsUsedAsync(Token token)
        {
            // TODO: Unit test.
            var filterBuilder = new FilterDefinitionBuilder<Token>();
            var updateBuilder = new UpdateDefinitionBuilder<Token>();
            await _collection.UpdateOneAsync(filterBuilder.Eq(i => i.Id, token.Id),
                updateBuilder.Set(i => i.TimeUsed, DateTime.UtcNow));
        }
    }
}