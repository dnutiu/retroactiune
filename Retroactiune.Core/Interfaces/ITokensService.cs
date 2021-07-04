using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Services;

namespace Retroactiune.Core.Interfaces
{
    public interface ITokensService
    {
        /// <summary>
        /// Generates tokens.
        /// </summary>
        /// <param name="numberOfTokens">The number of tokens to generate.</param>
        /// <param name="feedbackReceiverGuid">The feedback receiver guid to reference the tokens.</param>
        /// <param name="expiryTime">Optional expiry time for the tokens.</param>
        /// <returns>The result of the generate operation.</returns>
        public Task GenerateTokensAsync(int numberOfTokens, string feedbackReceiverGuid,
            DateTime? expiryTime = null);

        /// <summary>
        /// Deletes tokens.
        /// </summary>
        /// <param name="tokenIds">A list of tokens to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        public Task DeleteTokens(IEnumerable<string> tokenIds);

        /// <summary>
        /// List and filters tokens.
        /// </summary>
        /// <param name="filters">Filters object for filtering results.</param>
        /// <returns>A list of tokens matching the filters.</returns>
        public Task<IEnumerable<Token>> ListTokens(TokenListFilters filters);

        /// <summary>
        /// Deletes tokens, by their associated FeedbackReceiverId.
        /// </summary>
        /// <param name="feedbackReceiverIds">A list of FeedbackReceiverIDs.</param>
        /// <returns>The result of the delete operation.</returns>
        public Task DeleteManyByFeedbackReceiverIdAsync(IEnumerable<string> feedbackReceiverIds);
    }
}