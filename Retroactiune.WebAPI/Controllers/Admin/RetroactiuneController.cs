using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Retroactiune.Models;

namespace Retroactiune.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class RetroActiune : ControllerBase
    {
        
        [HttpDelete("{id}")]
        public NoContentResult Delete(long id)
        {
            return NoContent();
        }
        
        
        [HttpPost]
        public BasicResponse Post()
        {
            return new BasicResponse()
            {
                Message = "post retroactiune"
            };
        }
        
        [HttpGet("{id}")]
        public BasicResponse Get(long id)
        {
            return new BasicResponse()
            {
                Message = "hwlo"
            };
        }
        
        [HttpGet]
        public IEnumerable<BasicResponse> List()
        {
            return Enumerable.Range(1, 5).Select(i =>
                new BasicResponse()
                {
                    Message = "hwlo"
                }
            );
        }
    }
}