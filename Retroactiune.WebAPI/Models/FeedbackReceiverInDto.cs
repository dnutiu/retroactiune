using System.ComponentModel.DataAnnotations;

namespace Retroactiune.Models
{
    /// <summary>
    /// FeedbackReceiverInDto is the DTO for <see cref="FeedbackReceiver"/>, used in incoming requests.
    /// </summary>
    public class FeedbackReceiverInDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
    
    /// <summary>
    /// FeedbackReceiverDto is the DTO for <see cref="FeedbackReceiver"/>, used in outgoing requests.
    /// </summary>
    public class FeedbackReceiverOutDto : FeedbackReceiver
    {
    }
}