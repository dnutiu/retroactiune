using System;

namespace Retroactiune.DataTransferObjects
{
    /// <summary>
    /// DTO with filters for listing Feedbacks.
    /// </summary>
    public class ListFeedbacksFiltersDto
    {
        public uint Rating { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}