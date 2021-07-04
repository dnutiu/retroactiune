using System;
using System.ComponentModel.DataAnnotations;

namespace Retroactiune.DataTransferObjects
{
    /// <summary>
    /// FeedbackInDto is used by users for submitting feedback.
    /// </summary>
    public class FeedbackInDto
    {
        private uint _rating;

        [Required, StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
        public string FeedbackReceiverId { get; set; }

        [Required]
        public uint Rating
        {
            get => _rating;
            set
            {
                if (value <= 5)
                {
                    _rating = value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        [Required] public string Description { get; set; }
    }
}