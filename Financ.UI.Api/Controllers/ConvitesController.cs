using Financ.Application.CQRS.Commands;
using Financ.Application.CQRS.Query;
using Financ.Application.DTOs.Convites.Post;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConvitesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ConvitesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> ConvidaUsuario(CriaConviteDTO conviteDTO)
        {
            var convite = await _mediator.Send(new CriaConviteCommand(User.RetornaIdUsuario(), conviteDTO.EmailDestinatario, conviteDTO.IdConta, conviteDTO.Acesso));
            return convite.RetornoAutomatico();
        }
        [HttpGet]
        public async Task<IActionResult> RetornaConvites([FromQuery]bool remetente)
        {
            var convite = await _mediator.Send(new RetornaConvitesQuery(User.RetornaIdUsuario(), remetente));
            return convite.RetornoAutomatico();
        }
        [HttpPost("{id}/revogar")]
        public async Task<IActionResult> RevogarConvites([FromRoute] int id)
        {
            var convite = await _mediator.Send(new RevogaConviteCommand(id, User.RetornaIdUsuario()));
            return convite.RetornoAutomatico();
        }
        [HttpPost("entrar")]
        public async Task<IActionResult> EntrarNaConta(int idConvite, bool aceito)
        {
            var usuarioConta = await _mediator.Send(new IncluiUsuarioContaCommand(idConvite, aceito, User.RetornaIdUsuario()));
            return usuarioConta.RetornoAutomatico();
        }
    }
}
