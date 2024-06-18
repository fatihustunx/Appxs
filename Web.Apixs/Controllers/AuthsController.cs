using Features.Features.Auths.Auths;
using Features.Features.Auths.Roles;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Web.Apixs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthsFeatures _authsFeatures;
        private readonly IMediator _mediator;
        public AuthsController(IAuthsFeatures authsFeatures,
            IMediator mediator)
        {
            _authsFeatures = authsFeatures;
            _mediator = mediator;
        }

        [HttpPost("GirişYap")]
        public async Task<ActionResult> GirisYap(UserForLoginDto userForLoginDto)
        {
            var res = await _authsFeatures.RunToLogin(userForLoginDto);

            return Ok(res);
        }

        [HttpPost("KayıtOl")]
        public async Task<ActionResult> KayitOl(UserForRegisterDto userForRegisterDto)
        {
            var res = await _authsFeatures.RunToRegister(userForRegisterDto);

            return Ok(res);
        }

        [HttpPost("BirRolVer")]
        public async Task<ActionResult> BirRolVer(BirRolVerRequest birRolVerRequest)
        {
            var res = await _mediator.Send(birRolVerRequest);

            return Ok(res);
        }

        [HttpGet("Kullanıcılar")]
        public async Task<ActionResult> Kullanicilar()
        {
            var res = await _mediator.Send(new UsersRequest()); return Ok(res);
        }

        [HttpGet("Roller")]
        public async Task<ActionResult> Roller()
        {
            var res = await _mediator.Send(new RolesRequest()); return Ok(res);
        }
    }
}