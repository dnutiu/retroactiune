using System;

namespace Retroactiune.Core.Services
{
    /// <summary>
    /// FeedbacksListFilters is a data class for filtering Feedbacks.
    /// </summary>
    public class FeedbacksListFilters
    {
        
        /// <summary>
        /// FeedbackReceiverId the ID of the FeedbackReceiver.
        /// </summary>
        public string FeedbackReceiverId { get; set; }
        
        /// <summary>
        /// CreatedAfter filters items that have been created after the given date.
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// CreatedBefore filters items that have been created before the given date.
        /// </summary>
        public DateTime? CreatedBefore { get; set; }

        /// <summary>
        /// Rating filters for the rating.
        /// </summary>
        public uint Rating { get; set; }
    }
}