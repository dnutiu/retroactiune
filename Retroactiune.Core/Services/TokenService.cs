using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;

namespace Retroactiune.Core.Services
{
    public class TokenService : ITokensService
    {
        private readonly IMongoCollection<Token> _collection;

        public TokenService(IMongoClient client, IDatabaseSettings settings)
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
            var filterBuilder = new FilterDefinitionBuilder<Token>();
            var activeFilters = new List<FilterDefinition<Token>>();
            var tokensListFilter = FilterDefinition<Token>.Empty;

            // Filter by token ids.
            if (filters.Ids.Any())
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
    }
}