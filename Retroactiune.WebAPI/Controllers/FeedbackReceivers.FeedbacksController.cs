using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;

namespace Retroactiune.Controllers
{
    /// <summary>
    /// Implementation FeedbackReceiversController and Feedbacks related functionality.
    /// </summary>
    public partial class FeedbackReceiversController
    {
        
        /// <summary>
        /// Add Feedback to a FeedbackReceiver.
        /// </summary>
        /// <param name="feedbackInDto">The feedback dto.</param>
        /// <response code="200">The feedback has been added.</response>
        /// <response code="400">The request is invalid.</response>  
        /// <returns></returns>
        [HttpPost("feedbacks")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackInDto feedbackInDto)
        {
            var tokenEnum = await _tokensService.FindAsync(new TokenListFilters
            {
                Ids = new[] {feedbackInDto.TokenId}
            });
            var tokens = (tokenEnum as Token[] ?? tokenEnum.ToArray());


            if (tokens.Length == 0)
            {
                return BadRequest(new BasicResponse
                {
                    Message = "Token not found."
                });
            }

            var token = tokens[0];

            var receivers = await _feedbackReceiversService.FindAsync(
                new[] {token.FeedbackReceiverId}, limit: 1
            );
            var feedbackReceivers = receivers as FeedbackReceiver[] ?? receivers.ToArray();
            if (!feedbackReceivers.Any())
            {
                return BadRequest(new BasicResponse
                {
                    Message = $"FeedbackReceiver with id {token.FeedbackReceiverId} not found."
                });
            }

            if (!token.IsValid(feedbackReceivers[0]))
            {
                return BadRequest(new BasicResponse
                {
                    Message = "Token is invalid."
                });
            }

            var feedback = _mapper.Map<Feedback>(feedbackInDto);
            await Task.WhenAll(_tokensService.MarkTokenAsUsedAsync(token),
                _feedbacksService.AddFeedbackAsync(feedback, feedbackReceivers[0]));
            return Ok();
        }

        /// <summary>
        /// Returns the Feedbacks of a FeedbackReceiver. See <see cref="Feedback"/> and <see cref="FeedbackReceiver"/>.
        /// </summary>
        /// <param name="guid">The guid of the FeedbackReceiver.</param>
        /// <param name="filters">Query filters for filtering the response.</param>
        /// <response code="200">The feedback has been added.</response>
        /// <response code="400">The request is invalid.</response>  
        /// <returns></returns>
        [HttpGet("{guid}/feedbacks")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetFeedbacks(string guid, [FromQuery] ListFeedbacksFiltersDto filters)
        {
            // TODO: Unit & Integration test.
            var feedbacksListFilters = _mapper.Map<FeedbacksListFilters>(filters);
            feedbacksListFilters.FeedbackReceiverId = guid;
            var response = await _feedbacksService.GetFeedbacksAsync(feedbacksListFilters);
            return Ok(response);
        }
    }
}