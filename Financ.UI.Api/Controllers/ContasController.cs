using Financ.Application.CQRS.Contas_.Commands;
using Financ.Application.CQRS.Contas_Commands;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.Contas.Ptch;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [HttpPost]
        public async Task<IActionResult> CadastrarContas(CadastrarContaDTO contaDTO)
        {
            var conta = await _mediator.Send(new CriarContaCommand(User.RetornaIdUsuario(), contaDTO.Titulo));
            return conta.RetornoAutomatico();
        }

        [HttpPatch("{idContaUsuario}/atualiza")]
        public async Task<IActionResult> AtualizaConta(int idContaUsuario, AtualizaContaDTO contaDTO)
        {

            var contaAtualizada = await _mediator.Send(new AtualizarContaCommand(idContaUsuario, User.RetornaIdUsuario(), contaDTO.Status, contaDTO.Titulo));
            return contaAtualizada.RetornoAutomatico();
        }
    }
}
