using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Retroactiune.Core.Entities;

namespace Retroactiune.DataTransferObjects
{
    /// <summary>
    /// DTO with filters for listing tokens. <see cref="Token"/>
    /// </summary>
    public class ListTokensFiltersDto
    {
        public IEnumerable<string> Ids { get; set; }
        [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
        public string FeedbackReceiverId { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? UsedAfter { get; set; }
        public DateTime? UsedBefore { get; set; }
    }
}