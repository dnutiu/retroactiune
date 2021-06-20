using System.Collections.Generic;
using System.Threading.Tasks;
using Retroactiune.Models;

namespace Retroactiune.Services
{
    public interface IFeedbackReceiverService
    {
        /// <summary>
        /// Creates feedback receivers.
        /// </summary>
        /// <param name="items">A collection of feedback receivers.</param>
        /// <returns>Nothing.</returns>
        public Task CreateManyAsync(IEnumerable<FeedbackReceiver> items);

        /// <summary>
        /// Deletes one FeedbackReceiver.
        /// </summary>
        /// <param name="guids">A list of FeedbackReceiver guids to delete.</param>
        /// <returns>Nothing.</returns>
        public Task DeleteManyAsync(IEnumerable<string> guids);

        /// <summary>
        /// Finds FeedbackReceivers.
        /// </summary>
        /// <param name="guid">A list of guids to filter the FeedbackReceivers. </param>
        /// <param name="offset">An offset, it skips the specified FeedbackReceivers.</param>
        /// <param name="limit">A limit for the returned results.</param>
        /// <returns>A collection of FeedbackReceivers.</returns>
        Task<IEnumerable<FeedbackReceiver>> FindAsync(IEnumerable<string> guid, int? offset = null, int? limit = null);
    }
}