using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retroactiune.DataTransferObjects;
using Retroactiune.Services;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TokensController : ControllerBase
    {
        // TODO: Add tokens, list tokens (unused, used), delete tokens.

        private readonly IFeedbackReceiverService _feedbackReceiverService;
        private readonly ITokensService _tokensService;

        public TokensController(IFeedbackReceiverService feedbackReceiverService, ITokensService tokensService)
        {
            _feedbackReceiverService = feedbackReceiverService;
            _tokensService = tokensService;
        }

        // TODO: Test integration.
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

            await _tokensService.GenerateTokensAsync(generateTokensDto.NumberOfTokens, feedbackReceiverId, generateTokensDto.ExpiryTime);
            return Ok(new BasicResponse
            {
                Message = "Tokens generated."
            });
        }
    }
}