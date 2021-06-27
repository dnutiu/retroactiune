using System;
using System.Collections.Generic;

namespace Retroactiune.Core.Services
{
    /// <summary>
    /// TokenListFilters is a data class representing the filters used to list tokens.
    /// </summary>
    public class TokenListFilters
    {
        /// <summary>
        /// Id filters tokens by their ids.
        /// </summary>
        public IEnumerable<string> Ids { get; set; }

        /// <summary>
        /// FeedbackReceiverId filters tokens by their assigned FeedbackReceiverId.
        /// </summary>
        public string FeedbackReceiverId { get; set; }

        /// <summary>
        /// CreatedAfter filters token that have been created after the given date.
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// CreatedBefore filters token that have been created before the given date.
        /// </summary>
        public DateTime? CreatedBefore { get; set; }

        /// <summary>
        /// UsedAfter filters token that have been used after the given date.
        /// </summary>
        public DateTime? UsedAfter { get; set; }

        /// <summary>
        /// UsedBefore filters token that have been used before the given date.
        /// </summary>
        public DateTime? UsedBefore { get; set; }
    }
}