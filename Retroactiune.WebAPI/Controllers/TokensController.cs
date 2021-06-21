using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TokensController : ControllerBase
    {
        // TODO: Implement ListTokens.
        // Filters for: FeedbackReceiver IDS
        // for: start < CreatedTime < end
        // for start < TimeUsed end

        private readonly IFeedbackReceiverService _feedbackReceiverService;
        private readonly ITokensService _tokensService;

        public TokensController(IFeedbackReceiverService feedbackReceiverService, ITokensService tokensService)
        {
            _feedbackReceiverService = feedbackReceiverService;
            _tokensService = tokensService;
        }

        /// <summary>
        /// Creates a new batch of tokens, the tokens are tied to a FeedbackReceiver and are used by the client
        /// when leaving Feedback.
        /// </summary>
        /// <param name="generateTokensDto">The list of FeedbackReceivers</param>
        /// <returns>A BasicResponse indicating success.</returns>
        /// <response code="200">Returns ok.</response>
        /// <response code="400">If the items is invalid</response>  
        [HttpPost]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateTokens([Required] GenerateTokensDto generateTokensDto)
        {
            var feedbackReceiverId = generateTokensDto.FeedbackReceiverId;
            var result = await _feedbackReceiverService.FindAsync(
                new[] {feedbackReceiverId});
            if (!result.Any())
            {
                return BadRequest(new BasicResponse
                {
                    Message = $"Invalid FeedbackReceiverId {generateTokensDto.FeedbackReceiverId}."
                });
            }

            await _tokensService.GenerateTokensAsync(generateTokensDto.NumberOfTokens, feedbackReceiverId,
                generateTokensDto.ExpiryTime);
            return Ok(new BasicResponse
            {
                Message = "Tokens generated."
            });
        }

        /// <summary>
        /// Deletes tokens identified by ids.
        /// </summary>
        /// <param name="tokenIds">A list of token ids.</param>
        /// <response code="204">The request to delete the items has been submitted.</response>
        /// <response code="404">The request is invalid.</response>  
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTokens([Required] IEnumerable<string> tokenIds)
        {
            try
            {
                await _tokensService.DeleteTokens(tokenIds);
                return NoContent();
            }
            catch (GenericServiceException e)
            {
                return BadRequest(new BasicResponse
                {
                    Message = e.Message
                });
            }
        }
        
        /// <summary>
        /// Deletes a Token given it's guid.
        /// </summary>
        /// <param name="guid">The guid of the item to be deleted.</param>
        /// <returns>A NoContent result.</returns>
        /// <response code="204">The delete is submitted.</response>
        /// <response code="400">The request is invalid.</response>  
        [HttpDelete("{guid}")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteToken(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)] string guid)
        {
            try
            {
                await _tokensService.DeleteTokens(new []{ guid });
                return NoContent();
            }
            catch (GenericServiceException e)
            {
                return BadRequest(new BasicResponse
                {
                    Message = e.Message
                });
            }
        }
    }
}