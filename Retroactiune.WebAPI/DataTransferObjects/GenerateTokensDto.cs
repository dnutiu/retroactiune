using System;
using System.ComponentModel.DataAnnotations;
using Retroactiune.DataAnnotations;

namespace Retroactiune.DataTransferObjects
{
    /// <summary>
    /// GenerateTokensDto is the payload that is sent by the user for generating tokens.
    /// </summary>
    public class GenerateTokensDto
    {
        [Range(1, 1000, ErrorMessage = "numberOfTokens is  out of range, allowed ranges [1-1000]")]
        public int NumberOfTokens { get; set; } = 1;

        [Required, StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
        public string FeedbackReceiverId { get; set; }

        [DatetimeNotInThePast(ErrorMessage = "expiryTime cannot be in the past!")]
        public DateTime? ExpiryTime { get; set; } = null;
    }
}