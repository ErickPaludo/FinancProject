using Financ.Application.CQRS.Fixas.Commands;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Movimentações.Fixas.Post;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.Movimentações.Fixas;
using Financ.UI.Api.Extensao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetDevPack.SimpleMediator;

namespace Financ.UI.Api.Controllers
{
    [Route("api/Contas/{idConta}/Movimentacoes/Fixa")]
    [ApiController]
    [Authorize]
    public class MovimentacaoFixaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MovimentacaoFixaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CriaFixo(int idConta, CriaMovimentacaoFixaDTO fixoDTO)
        {
            var movFixo = await  _mediator.Send(new CriaMovimentacaoFixaCommand
            {
                idConta = idConta,
                idUsuario = User.RetornaIdUsuario(),
                IdsCategoria = fixoDTO.Movimentacao.IdsCategoria,
                tipo = fixoDTO.Movimentacao.tipo,
                valor = fixoDTO.Movimentacao.valor,
                titulo = fixoDTO.Movimentacao.titulo,
                observacao = fixoDTO.Movimentacao.observacao,
                DataInicio = fixoDTO.DataInicio,
                DataFim = fixoDTO.DataFim,
                DataOcorrencia = fixoDTO.DataOcorrencia,
                TipoFixo = fixoDTO.Tipo
            });
            return movFixo.RetornoAutomatico();
        }
        [HttpPost("Diarias")]
        public async Task<IActionResult> CriaFixoDiaria(int idConta, CriaMovimentacaoFixaDiariaDTO fixoDTO)
        {
            var movFixo = await  _mediator.Send(new CriaMovimentacaoFixaDiariaCommand
            {
                idConta = idConta,
                idUsuario = User.RetornaIdUsuario(),
                IdsCategoria = fixoDTO.Movimentacao.IdsCategoria,
                tipo = fixoDTO.Movimentacao.tipo,
                valor = fixoDTO.Movimentacao.valor,
                titulo = fixoDTO.Movimentacao.titulo,
                observacao = fixoDTO.Movimentacao.observacao,
                DataInicio = fixoDTO.DataInicio,
                DataFim = fixoDTO.DataFim,
                OcorrenciasDiarias = fixoDTO.OcorrenciaDiaria,
            });
            return movFixo.RetornoAutomatico();
        }

        [HttpPost("{idFixo}/Materializa")]
        public async Task<IActionResult> MaterializaFixo(int idConta,int idFixo,[FromBody] MaterializaMovimentacaoFixa movimentacaoDTO)
        {
            var movFixo = await _mediator.Send(new MaterializaMovimentacaoFixaCommand(idFixo,User.RetornaIdUsuario(),movimentacaoDTO.DataMovimentacao));
            return movFixo.RetornoAutomatico();
        }
        [HttpGet]
        public async Task<IActionResult> RetornaFixos(int idConta, [FromQuery] TipoMovimentacaoFixa? tipoMovimentacao)
        {
            var movFixo = await _mediator.Send(new RetornarMovimentacoesFixasCommand(idConta, User.RetornaIdUsuario(), tipoMovimentacao));
            return movFixo.RetornoAutomatico();
        }
    }
}
