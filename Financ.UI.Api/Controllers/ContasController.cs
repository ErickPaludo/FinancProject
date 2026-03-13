using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_.Commands;
using Financ.Application.CQRS.Contas_.Querys;
using Financ.Application.CQRS.Contas_Commands;
using Financ.Application.DTOs.Contas.Get;
using Financ.Application.DTOs.Contas.Get.Filtros;
using Financ.Application.DTOs.Contas.Ptch;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Autenticação;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;
using System.Security.Claims;

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
        [HttpGet]
        public async Task<IActionResult> RetornarContas([FromQuery] FiltroContaDTO? parametros)
        {
            var contasLista = await _mediator.Send(new RetornaContaQuery(User.RetornaIdUsuario(), parametros));
            return contasLista.RetornoAutomatico();
        }

        [HttpPatch("{idContaUsuario}/atualiza")]
        public async Task<IActionResult> AtualizaConta(int idContaUsuario, AtualizaContaDTO contaDTO)
        {

            var contaAtualizada = await _mediator.Send(new AtualizarContaCommand(idContaUsuario, User.RetornaIdUsuario(), contaDTO.Status, contaDTO.Titulo));
            return contaAtualizada.RetornoAutomatico();
        }
    }
}
