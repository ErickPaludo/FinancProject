using Financ.Application.CQRS.Contas_Usuarios.Commands;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.CQRS.ContasUsuarios.Commands;
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
        public async Task<IActionResult> RetornarContas([FromQuery] FiltroContasUsuarioDTO? parametros)
        {
            var contasLista = await _mediator.Send(new RetornaContaUsuariosQuery(User.RetornaIdUsuario(), parametros));
            return contasLista.RetornoAutomatico();
        }

        [HttpGet("{idConta}/associados")]
        public async Task<IActionResult> RetornaUsuarosAssociados([FromRoute]int idConta, [FromQuery] FiltroUsuarioAssociado? filtroConta = null)
        {
            var usuariosAssociados = await _mediator.Send(new RetornaUsuariosAssociadosQuery(idConta,User.RetornaIdUsuario(), filtroConta));
            return usuariosAssociados.RetornoAutomatico();
        }

        [HttpPatch("{idConta}/alterar")]
        public async Task<IActionResult> AlteraUsuarioConta([FromRoute]int idConta, [FromBody] AtualizaContasUsuariosDTO contaUsuario)
        {
            var usuarioAlterado = await _mediator.Send(new AtualizarContaUsuarioCommand(User.RetornaIdUsuario(), contaUsuario.idUsuarioAlterado!, idConta, contaUsuario.acesso, contaUsuario.status,contaUsuario.expiracao,contaUsuario.removerExpiracao));
            return usuarioAlterado.RetornoAutomatico();
        }
       
        [HttpPost("{idConta}/sair")]
        public async Task<IActionResult> SairDaConta([FromRoute]int idConta)
        {
            var convite = await _mediator.Send(new SairContaUsuarioCommand( User.RetornaIdUsuario(),idConta));
            return convite.RetornoAutomatico();
        }

        [HttpPost("{idConta}/expurgo")]
        public async Task<IActionResult> RemoveContaUsuario([FromRoute]int idConta, [FromBody]RemoveContaUsuarioDTO contaUsuarioDTO)
        {
            var convite = await _mediator.Send(new RemoveContaUsuarioCommand(User.RetornaIdUsuario(),contaUsuarioDTO.idUsuarioDestinatario,idConta));
            return convite.RetornoAutomatico();
        }

        [HttpPost("{idConta}/Favorita")]
        public async Task<IActionResult> ContaFavorita([FromRoute] int idConta)
        {
            var conta = await _mediator.Send(new FavoritaContaUsuarioCommand(idConta, User.RetornaIdUsuario()));
            return conta.RetornoAutomatico();
        }
    
        [HttpPost("{idConta}/AutoSoma")]
        public async Task<IActionResult> ContaAutoSoma([FromRoute] int idConta)
        {
            var conta = await _mediator.Send(new AutoSomaContaUsuarioCommand(idConta, User.RetornaIdUsuario()));
            return conta.RetornoAutomatico();
        }
    }
}
