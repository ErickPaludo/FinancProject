using Financ.Application.CQRS.Commands;
using Financ.Application.CQRS.Query;
using Financ.Application.CQRS.Querys;
using Financ.Application.DTOs.Autenticação.Post;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Application.DTOs.ContasUsuarios.Patch;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.DTOs.Convites.Post;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Repositorios;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Financ.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContasUsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ContasUsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("entrar_na_conta")]
        public async Task<IActionResult> EntrarNaConta(int idConvite,bool aceito)
        {
            var usuarioConta = await _mediator.Send(new IncluiUsuarioContaCommand(idConvite,aceito, User.RetornaIdUsuario()));
            return usuarioConta.RetornoAutomatico();
        }

        [HttpGet("retorna_usuarios_associados")]
        public async Task<IActionResult> RetornaUsuarosAssociados([FromQuery] FiltroUsuarioAssociado filtroConta)
        {
            var usuariosAssociados = await _mediator.Send(new RetornaUsuariosAssociadosQuery(User.RetornaIdUsuario(), filtroConta.IdConta, filtroConta.IdUsuario, filtroConta.NomeUsuario, filtroConta.Acesso, filtroConta.Status));
            return usuariosAssociados.RetornoAutomatico();
        }

        [HttpPatch("altera_usuario_conta")]
        public async Task<IActionResult> AlteraUsuarioConta(int idConta, string idUsuario, [FromBody] AtualizaContasUsuariosDTO contaUsuario)
        {
            var usuarioAlterado = await _mediator.Send(new AtualizarContaUsuarioCommand(User.RetornaIdUsuario(), idUsuario, idConta, contaUsuario.Acesso, contaUsuario.Status));
            return usuarioAlterado.RetornoAutomatico();
        }

        [HttpPost("convida_usuario")]
        public async Task<IActionResult> ConvidaUsuario(CriaConviteDTO conviteDTO)
        {
            var convite = await _mediator.Send(new CriaConviteCommand(User.RetornaIdUsuario(), conviteDTO.EmailDestinatario, conviteDTO.IdConta, conviteDTO.Acesso));
            return convite.RetornoAutomatico();
        } 
        [HttpGet("retorna_convites")]
        public async Task<IActionResult> RetornaConvites(bool retornaConvitesRemetente)
        {
            var convite = await _mediator.Send(new RetornaConvitesQuery(User.RetornaIdUsuario(), retornaConvitesRemetente));
            return convite.RetornoAutomatico();
        }
        [HttpPost("revogar_convite")]
        public async Task<IActionResult> RevogarConvites(int idConvite)
        {
            var convite = await _mediator.Send(new RevogaConviteCommand(idConvite, User.RetornaIdUsuario()));
            return convite.RetornoAutomatico();
        }
        [HttpPost("sair_conta")]
        public async Task<IActionResult> SairDaConta(int idConta)
        {
            var convite = await _mediator.Send(new SairContaUsuarioCommand( User.RetornaIdUsuario(),idConta));
            return convite.RetornoAutomatico();
        }
    }
}
