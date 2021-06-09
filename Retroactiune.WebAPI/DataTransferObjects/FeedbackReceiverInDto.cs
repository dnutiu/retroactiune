using System.ComponentModel.DataAnnotations;
using Retroactiune.Models;

namespace Retroactiune.DataTransferObjects
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
}