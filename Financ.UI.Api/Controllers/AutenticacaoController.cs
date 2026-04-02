using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Autenticação.Commands;
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
            var tokenAutenticacao = await _mediator.Send(new AutenticacaoCommand(usuario.Email, usuario.Senha));

            if (tokenAutenticacao.ValidaSucesso)
                SetRefreshTokenCookie(tokenAutenticacao!.Sucesso!.RefreshToken);

            return tokenAutenticacao.RetornoAutomatico();
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var tokenAutenticacao = await _mediator.Send(new RefreshTokenCommand(refreshToken));

            if (tokenAutenticacao.ValidaSucesso)
                SetRefreshTokenCookie(tokenAutenticacao!.Sucesso!.RefreshToken);

            return tokenAutenticacao.RetornoAutomatico();
        }
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            var tokenAutenticacao = await _mediator.Send(new RevokeCommand(User.RetornaIdUsuario()));
            return tokenAutenticacao.RetornoAutomatico();
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken)) return;

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // OBRIGATÓRIO PARA HTTPS
                SameSite = SameSiteMode.None, // OBRIGATÓRIO PARA CROSS-DOMAIN
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
