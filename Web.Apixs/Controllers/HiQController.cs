using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Apixs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HiQController : ControllerBase
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hi Q,");
        }
    }
}