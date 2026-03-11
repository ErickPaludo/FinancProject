using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.DTOs.Autenticação.Post;
using Financ.Domain.Interfaces.Autenticação;
using Financ.UI.Api.Extensao;
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
    }
}
