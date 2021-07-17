using System.Collections.Generic;
using System.Threading.Tasks;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Services;

namespace Retroactiune.Core.Interfaces
{
    public interface IFeedbacksService
    {

        /// <summary>
        /// Adds Feedback to a FeedbackReceiver.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        /// <param name="receiver">The feedback receiver.</param>
        /// <returns></returns>
        public Task AddFeedbackAsync(Feedback feedback, FeedbackReceiver receiver);

        /// <summary>
        /// Gets Feedbacks.
        /// </summary>
        /// <param name="filters">Filters for filtering the response.</param>
        /// <returns>An enumerable of feedbacks.</returns>
        Task<IEnumerable<Feedback>> GetFeedbacksAsync(FeedbacksListFilters filters);
    }
}