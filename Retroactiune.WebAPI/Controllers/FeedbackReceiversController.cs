using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Retroactiune.Models;
using Retroactiune.Services;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FeedbackReceiversController : ControllerBase
    {
        private readonly IOptions<ApiBehaviorOptions> _apiBehaviorOptions;
        private readonly IFeedbackReceiverService _service;
        private readonly IMapper _mapper;

        public FeedbackReceiversController(IFeedbackReceiverService service, IMapper mapper,
            IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            _service = service;
            _mapper = mapper;
            _apiBehaviorOptions = apiBehaviorOptions;
        }


        /// <summary>
        /// Inserts FeedbackReceiver items into the database.
        /// </summary>
        /// <param name="items">The list of FeedbackReceivers</param>
        /// <returns>A BasicResponse indicating success.</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the items is invalid</response>  
        [HttpPost]
        [ProducesResponseType(typeof(BasicResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([Required] IEnumerable<FeedbackReceiverDto> items)
        {
            var feedbackReceiversDto = items.ToList();
            if (!feedbackReceiversDto.Any())
            {
                ModelState.AddModelError(nameof(IEnumerable<FeedbackReceiverDto>),
                    "At least one FeedbackReceiver item is required.");
                return _apiBehaviorOptions?.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            var mappedItems = feedbackReceiversDto.Select(i =>
            {
                var result = _mapper.Map<FeedbackReceiver>(i);
                result.CreatedAt = DateTime.UtcNow;
                return result;
            });

            await _service.CreateManyAsync(mappedItems);

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
        /// <response code="204">The delete is successful.</response>
        /// <response code="400">The delete is unsuccessful.</response>  
        [HttpDelete("{guid}")]
        [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<NoContentResult> Delete(
            [StringLength(24, ErrorMessage = "invalid guid, must be 24 characters", MinimumLength = 24)]
            string guid)
        {
            await _service.DeleteOneAsync(guid);
            return NoContent();
        }

        [HttpGet("{id}")]
        public BasicResponse Get(long id)
        {
            // get feedback item from db
            return new BasicResponse()
            {
                Message = "hwlo"
            };
        }

        [HttpGet]
        public IEnumerable<BasicResponse> List()
        {
            // list all feedback items.
            return Enumerable.Range(1, 5).Select(i =>
                new BasicResponse()
                {
                    Message = "hwlo"
                }
            );
        }
    }
}