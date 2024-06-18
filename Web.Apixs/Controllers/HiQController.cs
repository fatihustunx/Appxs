using Features.Features.AppEntitys;
using Microsoft.AspNetCore.Mvc;
using MediatR;

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

        [HttpGet("/Appxs")]
        public IActionResult Appxs()
        {
            return Ok("Hi Q,");
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppEntityFeatureRequest req)
        {
            var res = await _mediator.Send(req);

            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            AppEntityFeature2Request req = new() { id=id};

            var res = await _mediator.Send(req);

            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            AppEntityFeature3Request req = new() { };

            var res = await _mediator.Send(req);

            return Ok(res);
        }

        [HttpPut]
        public async Task<IActionResult> Update(AppEntityFeature4Request req)
        {
            var res = await _mediator.Send(req);

            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            AppEntityFeature5Request req = new()
            {
                id = id,
            };

            var res = await _mediator.Send(req);

            if(!res.res)
            {
                return BadRequest(res);
            }

            return Ok(res);
        }
    }
}