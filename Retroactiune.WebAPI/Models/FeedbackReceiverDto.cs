using System.ComponentModel.DataAnnotations;

namespace Retroactiune.Models
{
    /// <summary>
    /// FeedbackReceiverDto is the DTO for <see cref="FeedbackReceiver"/>
    /// </summary>
    public class FeedbackReceiverDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}