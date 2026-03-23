using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.DTOs.Autenticação.Post;
using Financ.Domain.Validacoes;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AutenticacaoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(ConectaUsuarioDTO usuario)
        {
            var tokenAutenticacao = await _mediator.Send(new AutenticadoUsuarioCommand(usuario.Email, usuario.Senha));         
            return tokenAutenticacao.RetornoAutomatico();
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromHeader]string refreshToken)
        {
            var tokenAutenticacao = await _mediator.Send(new RefreshTokenCommand(refreshToken));
            return tokenAutenticacao.RetornoAutomatico();
        }
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            var tokenAutenticacao = await _mediator.Send(new RevokeCommand(User.RetornaIdUsuario()));
            return tokenAutenticacao.RetornoAutomatico();
        }
    }
}
