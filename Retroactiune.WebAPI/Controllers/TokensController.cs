using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly IFeedbackReceiversService _feedbackReceiversService;
        private readonly ITokensService _tokensService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TokensController(IFeedbackReceiversService feedbackReceiversService, ITokensService tokensService,
            ILogger<TokensController> logger, IMapper mapper)
        {
            _feedbackReceiversService = feedbackReceiversService;
            _tokensService = tokensService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// The list tokens controller retrieves a list of tokens.
        /// </summary>
        /// <param name="filtersDto">Object that holds filters for listing tokens.</param>
        /// <response code="200">A list of tokens.</response>
        /// <response code="400">The request is invalid.</response>  
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<Token>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType( StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListTokens([FromQuery] ListTokensFiltersDto filtersDto)
        {
            try
            {
                var tokenFilters = _mapper.Map<TokenListFilters>(filtersDto);
                var response = await _tokensService.FindAsync(tokenFilters);
                return Ok(response);
            }
            catch (GenericServiceException e)
            {
                _logger.LogError("{Message}", e.Message);
                return BadRequest(new BasicResponse()
                {
                    Message = e.Message
                });
            }
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
        [Authorize]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType( StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GenerateTokens([Required] GenerateTokensDto generateTokensDto)
        {
            var feedbackReceiverId = generateTokensDto.FeedbackReceiverId;
            var result = await _feedbackReceiversService.FindAsync(
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
        [Authorize]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType( StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTokens([Required] IEnumerable<string> tokenIds)
        {
            try
            {
                await _tokensService.DeleteTokensAsync(tokenIds);
                return NoContent();
            }
            catch (GenericServiceException e)
            {
                _logger.LogError("{Message}", e.Message);
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
        [Authorize]
        [HttpDelete("{guid}")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType( StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteToken(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
            string guid)
        {
            try
            {
                await _tokensService.DeleteTokensAsync(new[] {guid});
                return NoContent();
            }
            catch (GenericServiceException e)
            {
                _logger.LogError("{Message}", e.Message);
                return BadRequest(new BasicResponse
                {
                    Message = e.Message
                });
            }
        }

        /// <summary>
        /// Checks if a token is valid or not.
        /// </summary>
        /// <response code="200">The the result of the check.</response>
        /// <response code="400">The request is invalid.</response>
        [HttpGet("{guid}/check")]
        [ProducesResponseType(typeof(CheckTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CheckToken(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
            string guid
        )
        {
            try
            {
                var response = await _tokensService.FindAsync(new TokenListFilters
                {
                    Ids = new[] {guid}
                });
                var token = response.ElementAt(0);
                return Ok(new CheckTokenDto
                {
                    IsValid = token.IsValid()
                });
            }
            catch (ArgumentOutOfRangeException)
            {
                _logger.LogWarning("Invalid token {Guid}", guid);
                return Ok(new CheckTokenDto
                {
                    IsValid = false
                });
            } 
        }
    }
}