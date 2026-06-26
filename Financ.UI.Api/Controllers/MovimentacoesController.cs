using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.CQRS.Movimentação.Handlers;
using Financ.Application.CQRS.Movimentação.Querys;
using Financ.Application.DTOs.ContasUsuarios.Put;
using Financ.Application.DTOs.Movimentações.Get.Filtros;
using Financ.Application.DTOs.Movimentações.Patch;
using Financ.Application.DTOs.Movimentações.Post;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/Contas/[controller]")]
    [ApiController]
    [Authorize]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MovimentacoesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("/api/Contas/{idConta}/[controller]")]
        public async Task<IActionResult> CriarMovimentacao(int idConta,CriaMovimentacaoDTO movimentacaoDTO)
        {
            var movimentacao = await _mediator.Send(new CriaMovimentacaoCommand{
                idConta = idConta,
                idUsuario = User.RetornaIdUsuario(),
                IdsCategoria = movimentacaoDTO.IdsCategoria,
                tipo = movimentacaoDTO.tipo,
                valor = movimentacaoDTO.valor,
                concluido = movimentacaoDTO.concluido,
                titulo = movimentacaoDTO.titulo,
                observacao = movimentacaoDTO.observacao,
                dthrMovimentacao = movimentacaoDTO.dthrMovimentacao,
                dthrConclusao = movimentacaoDTO.dthrConclusao
            });

            return Created();
        }
        [HttpPost("{idMovimentacao}/Concluir")]
        public async Task<IActionResult> ConcluirMovimentacao(int idMovimentacao, ConcluirMovimentacaoDTO movimentacaoDTO)
        {
            var movimentacao = await _mediator.Send(new ConcluirMovimentacaoCommand(User.RetornaIdUsuario(),idMovimentacao, movimentacaoDTO.dthrConclusao));

            return Ok(movimentacao);
        }
        [HttpPost("{idMovimentacao}/Extornar")]
        public async Task<IActionResult> ExtornarMovimentacao(int idMovimentacao)
        {
            var movimentacao = await _mediator.Send(new ExtornarMovimentacaoCommand(User.RetornaIdUsuario(), idMovimentacao));

            return Ok(movimentacao);
        }
        [HttpGet("/api/Contas/{idConta}/[controller]/Retornar")]
        public async Task<IActionResult> RetornaMovimentacao(int idConta, [FromQuery] FiltroRetornoMovimentacao filtro)
        {
            var movimentacao = await _mediator.Send(new RetornaMovimentacaoQuery(User.RetornaIdUsuario(), idConta,filtro));

            return movimentacao.RetornoAutomatico();
        }

        [HttpGet("/api/Contas/{idConta}/[controller]/Dash")]
        public async Task<IActionResult> RetornaMovimentacaoDash(int idConta)
        {
            var movimentacao = await _mediator.Send(new DashMovimentacoesTotaisCommand(idConta, User.RetornaIdUsuario()));

            return movimentacao.RetornoAutomatico();
        }
        [HttpPatch("{idMovimentacao}/Alterar")]
        public async Task<IActionResult> AlterarMovimentacao(int idMovimentacao,AlterarMovimentacaoDTO movimentacaoDTO)
        {
            var movimentacao = await _mediator.Send(new AlterarMovimentacaoCommand(
                idMovimentacao,
                User.RetornaIdUsuario(),
                movimentacaoDTO.titulo,
                movimentacaoDTO.observacao,
                movimentacaoDTO.tipo,
                movimentacaoDTO.valor,
                movimentacaoDTO.dthrMovimentacao,
                movimentacaoDTO.dthrConclusao));

            return movimentacao.RetornoAutomatico();
        }

        [HttpPut("{idMovimentacao}/Alterar/Categoria")]
        public async Task<IActionResult> AlterarCategoriaMovimentacao(int idMovimentacao, AlterarMovimentacaoCategoriaDTO movimentacaoDTO)
        {
            var movimentacao = await _mediator.Send(new AlterarCategoriaMovimentacaoCommand(idMovimentacao, User.RetornaIdUsuario(), movimentacaoDTO.categorias));
            return movimentacao.RetornoAutomatico();
        }
        [HttpDelete("{idMovimentacao}/Remover")]
        public async Task<IActionResult> RemoverMovimentacao(int idMovimentacao)
        {
            var movimentacao = await _mediator.Send(new RemoverMovimentacaoCommand(User.RetornaIdUsuario(), idMovimentacao));

            return movimentacao.RetornoAutomatico();
        }
    }
}
