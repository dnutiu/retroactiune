using System;
using System.Diagnostics.CodeAnalysis;

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

        public override bool Equals(object obj)
        {
            return obj != null && Equals((FeedbacksListFilters) obj);
        }

        private bool Equals(FeedbacksListFilters other)
        {
            return FeedbackReceiverId == other.FeedbackReceiverId &&
                   Nullable.Equals(CreatedAfter, other.CreatedAfter) &&
                   Nullable.Equals(CreatedBefore, other.CreatedBefore) && Rating == other.Rating;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            return HashCode.Combine(FeedbackReceiverId, CreatedAfter, CreatedBefore, Rating);
        }
    }
}