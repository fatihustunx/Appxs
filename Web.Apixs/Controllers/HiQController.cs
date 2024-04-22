using Features.Features;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Apixs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiQController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HiQController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hi Q,");
        }

        [HttpPost]
        public async Task<IActionResult> Get2(AppEntityFeatureRequest req)
        {
            var res = await _mediator.Send(req);

            return Ok(res);
        }

        [HttpPost("/2")]
        public async Task<IActionResult> Get3(AppEntityFeature2Request req)
        {
            var res = await _mediator.Send(req);

            return Ok(res);
        }
    }
}