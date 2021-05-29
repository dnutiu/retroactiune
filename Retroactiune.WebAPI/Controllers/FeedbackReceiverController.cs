using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Retroactiune.Models;
using Retroactiune.Services;

namespace Retroactiune.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FeedbackReceiverController : ControllerBase
    {
        private readonly FeedbackReceiverService _service;
        private readonly IMapper _mapper;

        public FeedbackReceiverController(FeedbackReceiverService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpDelete("{id}")]
        public NoContentResult Delete(long id)
        {
            // delete feedback item.
            return NoContent();
        }


        [HttpPost]
        public async Task<BasicResponse> Post(IEnumerable<FeedbackReceiverDto> items)
        {
            var mappedItems = items.ToList().Select(i =>
            {
                var result = _mapper.Map<FeedbackReceiver>(i);
                result.CreatedAt = DateTime.UtcNow;
                return result;
            });

            await _service.CreateMany(mappedItems);

            return new BasicResponse()
            {
                Message = "Items created successfully!"
            };
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