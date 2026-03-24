using Financ.Application.CQRS.Segurança.Commands;
using Financ.Application.CQRS.Usuarios.Commands;
using Financ.Application.CQRS.Usuarios.Querys;
using Financ.Application.DTOs.Usuarios.Post;
using Financ.Domain.Entidades;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsuariosController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar(CadastraUsuarioDTO usuarioDTO)
        {
            var usuario = await _mediator.Send(new CadastraUsuarioCommand(usuarioDTO.Email, usuarioDTO.PrimeiroNome, usuarioDTO.SegundoNome, usuarioDTO.Senha, usuarioDTO.ConfirmarSenha));
            return usuario.RetornoAutomatico();
        }
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> MeusDados()
        {
            var usuario = await _mediator.Send(new RetornaUsuarioPorIdQuery(User.RetornaIdUsuario()));
            return usuario.RetornoAutomatico();
        }

        [HttpPost("alterar_senha")]
        [Authorize]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaDTO senhaDTO)
        {
            var usuario = await _mediator.Send(new AlterarSenhaCommand(User.RetornaIdUsuario(), senhaDTO.senhaAntiga, senhaDTO.senhaNova));
            return usuario.RetornoAutomatico();
        }
    }
}
