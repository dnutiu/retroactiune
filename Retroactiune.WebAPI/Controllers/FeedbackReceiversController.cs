using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Retroactiune.Core.Entities;
using Retroactiune.Core.Interfaces;
using Retroactiune.Core.Services;
using Retroactiune.DataTransferObjects;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/feedback_receivers")]
    public class FeedbackReceiversController : ControllerBase
    {
        private readonly IOptions<ApiBehaviorOptions> _apiBehaviorOptions;

        // Note: Probably refactor this to use an Aggregate object, need to learn more about aggregates..
        private readonly IFeedbackReceiversService _feedbackReceiversService;
        private readonly ITokensService _tokensService;
        private readonly IFeedbacksService _feedbacksService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public FeedbackReceiversController(IFeedbackReceiversService feedbackReceiversService,
            ITokensService tokensService, IFeedbacksService feedbacksService, IMapper mapper,
            IOptions<ApiBehaviorOptions> apiBehaviorOptions, ILogger<FeedbackReceiversController> logger)
        {
            _feedbackReceiversService = feedbackReceiversService;
            _tokensService = tokensService;
            _feedbacksService = feedbacksService;
            _mapper = mapper;
            _apiBehaviorOptions = apiBehaviorOptions;
            _logger = logger;
        }


        /// <summary>
        /// Inserts FeedbackReceiver items into the database.
        /// </summary>
        /// <param name="items">The list of FeedbackReceivers</param>
        /// <returns>A BasicResponse indicating success.</returns>
        /// <response code="200">Returns an ok message.</response>
        /// <response code="400">If the items is invalid</response>  
        [HttpPost]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([Required] IEnumerable<FeedbackReceiverInDto> items)
        {
            var feedbackReceiversDto = items.ToList();
            if (!feedbackReceiversDto.Any())
            {
                ModelState.AddModelError(nameof(IEnumerable<FeedbackReceiverInDto>),
                    "At least one FeedbackReceiver item is required.");
                return _apiBehaviorOptions?.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            var mappedItems = feedbackReceiversDto.Select(i => _mapper.Map<FeedbackReceiver>(i));

            await _feedbackReceiversService.CreateManyAsync(mappedItems);

            return Ok(new BasicResponse()
            {
                Message = "Items created successfully!"
            });
        }

        /// <summary>
        /// Deletes a FeedbackReceiver item from the database.
        /// </summary>
        /// <param name="guid">The guid of the item to be deleted.</param>
        /// <returns>A NoContent result.</returns>
        /// <response code="204">The delete is submitted.</response>
        /// <response code="400">The request is invalid.</response>  
        [HttpDelete("{guid}")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<NoContentResult> Delete(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
            string guid)
        {
            await Task.WhenAll(_feedbackReceiversService.DeleteManyAsync(new[] {guid}),
                _tokensService.DeleteManyByFeedbackReceiverIdAsync(new[] {guid}));
            return NoContent();
        }

        /// <summary>
        /// Retrieves a FeedbackReceiver item from the database.
        /// </summary>
        /// <param name="guid">The guid of the item to be retrieved.</param>
        /// <returns>A Ok result with a <see cref="FeedbackReceiverOutDto"/>.</returns>
        /// <response code="200">The item returned successfully.</response>
        /// <response code="400">The request is invalid.</response>  
        /// <response code="404">The item was not found.</response>  
        [HttpGet("{guid}")]
        [ProducesResponseType(typeof(FeedbackReceiverOutDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
            string guid)
        {
            var result = await _feedbackReceiversService.FindAsync(new[] {guid});
            var feedbackReceivers = result as FeedbackReceiver[] ?? result.ToArray();
            if (!feedbackReceivers.Any())
            {
                return NotFound(new BasicResponse()
                {
                    Message = $"Item with guid {guid} was not found."
                });
            }

            return Ok(feedbackReceivers.First());
        }

        /// <summary>
        /// Retrieves a FeedbackReceiver items from the database.
        /// </summary>
        /// <param name="filter">If set, it will filter results for the given guids.</param>
        /// <param name="offset">If set, it will skip the N items. Allowed range is 1-IntMax.</param>
        /// <param name="limit">If set, it will limit the results to N items. Allowed range is 1-1000.</param>
        /// <returns>A Ok result with a list of <see cref="FeedbackReceiverOutDto"/>.</returns>
        /// <response code="200">The a list is returned.</response>
        /// <response code="400">The request is invalid.</response>  
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FeedbackReceiverOutDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] IEnumerable<string> filter,
            [RangeAttribute(1, int.MaxValue, ErrorMessage = "offset is  out of range, allowed ranges [1-IntMax]"),
             FromQuery]
            int offset,
            [RangeAttribute(1, 1000, ErrorMessage = "limit is  out of range, allowed ranges [1-1000]"), FromQuery]
            int limit)
        {
            return Ok(await _feedbackReceiversService.FindAsync(filter, offset, limit));
        }

        /// <summary>
        /// Deletes FeedbackReceiver identified by ids.
        /// </summary>
        /// <param name="ids">A list of FeedbackReceiver ids.</param>
        /// <response code="204">The request to delete the items has been submitted.</response>
        /// <response code="404">The request is invalid.</response>  
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMany([Required] IEnumerable<string> ids)
        {
            try
            {
                var feedbackReceiverIds = ids as string[] ?? ids.ToArray();
                await Task.WhenAll(_feedbackReceiversService.DeleteManyAsync(feedbackReceiverIds),
                    _tokensService.DeleteManyByFeedbackReceiverIdAsync(feedbackReceiverIds));
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
        /// Add Feedback to a FeedbackReceiver.
        /// </summary>
        /// <param name="guid">The guid of the FeedbackReceiver to add feedback.</param>
        /// <param name="feedbackInDto">The feedback dto.</param>
        /// <response code="200">The feedback has been added.</response>
        /// <response code="404">The request is invalid.</response>  
        /// <returns></returns>
        [HttpPost("{guid}/feedbacks")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddFeedback(string guid, [FromBody] FeedbackInDto feedbackInDto)
        {
            var receivers = await _feedbackReceiversService.FindAsync(new[] {guid}, limit: 1);
            var tokenEnum = await _tokensService.FindAsync(new TokenListFilters
            {
                Ids = new[] {feedbackInDto.TokenId}
            });
            var feedbackReceivers = receivers as FeedbackReceiver[] ?? receivers.ToArray();
            var tokens = (tokenEnum as Token[] ?? tokenEnum.ToArray());
            if (!feedbackReceivers.Any())
            {
                return BadRequest(new BasicResponse
                {
                    Message = $"FeedbackReceiver with id {guid} not found."
                });
            }

            if (tokens.Length == 0)
            {
                return BadRequest(new BasicResponse
                {
                    Message = "Token not found."
                });
            }

            var token = tokens[0];
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
        
        // TODO: Implement get for feedbacks.
    }
}