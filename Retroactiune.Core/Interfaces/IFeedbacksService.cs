using System.Threading.Tasks;
using Retroactiune.Core.Entities;

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
    }
}