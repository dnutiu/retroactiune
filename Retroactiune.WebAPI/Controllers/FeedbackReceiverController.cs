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
    public class FeedbackReceiverController : ControllerBase
    {
        private readonly IOptions<ApiBehaviorOptions> _apiBehaviorOptions;
        private readonly IFeedbackReceiverService _service;
        private readonly IMapper _mapper;

        public FeedbackReceiverController(IFeedbackReceiverService service, IMapper mapper,
            IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            _service = service;
            _mapper = mapper;
            _apiBehaviorOptions = apiBehaviorOptions;
        }


        /// <summary>
        /// Inserts FeedbackReceivers into the database.
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

        [HttpDelete("{id}")]
        public NoContentResult Delete(long id)
        {
            // delete feedback item.
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