using System.ComponentModel.DataAnnotations;

namespace Retroactiune.DataTransferObjects
{
    /// <summary>
    /// FeedbackInDto is used by users for submitting feedback.
    /// </summary>
    public class FeedbackInDto
    {
        
        [Required, StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)] 
        public string TokenId { get; set; }
        
        [Required, Range(0, 5, ErrorMessage = "The rating is out of range. [0-5]")]
        public uint Rating { get; set; }

        [Required] public string Description { get; set; }
    }
}