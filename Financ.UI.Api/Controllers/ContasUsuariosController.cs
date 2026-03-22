using Financ.Application.CQRS.Contas_Usuarios.Commands;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.DTOs.Autenticação.Post;
using Financ.Application.DTOs.ContasUsuarios.Get.Filtros;
using Financ.Application.DTOs.ContasUsuarios.Patch;
using Financ.Application.DTOs.ContasUsuarios.Post;
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

        [HttpGet]
        public async Task<IActionResult> RetornaUsuarosAssociados([FromQuery] FiltroUsuarioAssociado filtroConta)
        {
            var usuariosAssociados = await _mediator.Send(new RetornaUsuariosAssociadosQuery(User.RetornaIdUsuario(), filtroConta.IdConta, filtroConta.IdUsuario, filtroConta.NomeUsuario, filtroConta.Acesso, filtroConta.Status));
            return usuariosAssociados.RetornoAutomatico();
        }

        [HttpPatch("{idConta}/alterar")]
        public async Task<IActionResult> AlteraUsuarioConta([FromRoute]int idConta, [FromBody] AtualizaContasUsuariosDTO contaUsuario)
        {
            var usuarioAlterado = await _mediator.Send(new AtualizarContaUsuarioCommand(User.RetornaIdUsuario(), contaUsuario.idUsuarioAlterado!, idConta, contaUsuario.Acesso, contaUsuario.Status,contaUsuario.expiracao,contaUsuario.expirado));
            return usuarioAlterado.RetornoAutomatico();
        }
       
        [HttpPost("{idConta}/sair")]
        public async Task<IActionResult> SairDaConta([FromRoute]int idConta)
        {
            var convite = await _mediator.Send(new SairContaUsuarioCommand( User.RetornaIdUsuario(),idConta));
            return convite.RetornoAutomatico();
        }

        [HttpPost("expurgo")]
        public async Task<IActionResult> RemoveContaUsuario(RemoveContaUsuarioDTO contaUsuarioDTO)
        {
            var convite = await _mediator.Send(new RemoveContaUsuarioCommand(User.RetornaIdUsuario(),contaUsuarioDTO.idUsuarioDestinatario,contaUsuarioDTO.idConta));
            return convite.RetornoAutomatico();
        }
    }
}
