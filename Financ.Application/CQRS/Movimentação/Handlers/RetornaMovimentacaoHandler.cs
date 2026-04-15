using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Contas_Usuarios.Querys;
using Financ.Application.CQRS.Movimentação.Querys;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class RetornaMovimentacaoHandler : IRequestHandler<RetornaMovimentacaoQuery, Resultado<BaseGet<RetornaMovimentacaoDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RetornaMovimentacaoHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BaseGet<RetornaMovimentacaoDTO>>> Handle(RetornaMovimentacaoQuery request, CancellationToken cancellationToken)
        {
            Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);
            if(conta is null)
                return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada"));

            ContaUsuario? contaUsuario = conta!.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.IdUsuario);

            if(contaUsuario is null) return
                    Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraFalha(Falha.NaoEncontrado("Usuário não pertence a conta!"));

            contaUsuario!.ValidaSituacaoUsuarioParaConsulta();

            List<Movimentacao> movimentacoes = await MovimentacoesSelecionadas(request);

            decimal totalEntradaConcluidos = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is TipoStatusMovimentacao.Concluido).Sum(x => x.Valor);
            decimal totalSaidaConcluidos = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is TipoStatusMovimentacao.Concluido).Sum(x => x.Valor);

            decimal totalEntradaPendentes = movimentacoes.Where(x => x.Tipo is TipoMovimentacao.Entrada && x.Status is TipoStatusMovimentacao.Pendente).Sum(x => x.Valor);
            decimal totalSaidaPendentes = movimentacoes.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status is TipoStatusMovimentacao.Pendente).Sum(x => x.Valor);

            decimal totalEntrada = totalEntradaConcluidos + totalEntradaPendentes;
            decimal totalSaida = totalSaidaConcluidos + totalSaidaPendentes;

            decimal saldoRealizado = totalEntradaConcluidos - totalSaidaConcluidos;
            decimal saldoProjetado = (totalEntradaConcluidos + totalEntradaPendentes) - (totalSaidaConcluidos + totalSaidaPendentes);

            if (request.Filtros.Concluido.HasValue)
            {
                if (!request.Filtros.Concluido.Value)
                {
                    totalEntrada = totalEntradaPendentes;
                    saldoRealizado = 0;
                }
                else
                {
                    saldoRealizado = totalEntradaConcluidos - totalSaidaConcluidos;
                    saldoProjetado = 0;
                }
            }



            GrupoMovimentacaoDTO grupoEntrada = new GrupoMovimentacaoDTO(totalEntradaConcluidos, totalEntradaPendentes, totalEntrada);
            GrupoMovimentacaoDTO grupoSaida = new GrupoMovimentacaoDTO(totalSaidaConcluidos, totalSaidaPendentes, totalSaida);

            ResumoMovimentacoesDTO resumoDTO = new ResumoMovimentacoesDTO(saldoRealizado, saldoProjetado, grupoEntrada, grupoSaida);

            return Resultado<BaseGet<RetornaMovimentacaoDTO>>.GeraSucesso(new BaseGet<RetornaMovimentacaoDTO>(MovimentacaoMapper.ParaDTO(resumoDTO, await MovimentacoesSelecionadas(request))));
        }
        private async Task<List<Movimentacao>> MovimentacoesSelecionadas(RetornaMovimentacaoQuery request)
        {
            var filtro = request.Filtros;

            var queryable = _unitOfWork.movimentacaoRepositorio
                .BuscaMovimentacaoComContasUsuarios();

            queryable = queryable.Where(x => x.IdConta == request.IdConta && x.DthrMovimentacao >= filtro.DthrMovimentacaoInicial && x.DthrMovimentacao <= filtro.DthrMovimentacaoFinal);

            if (filtro?.IdMovimentacao.HasValue == true)
            {
                queryable = queryable.Where(x => x.Id == filtro.IdMovimentacao.Value);
            }

            if (filtro?.Concluido.HasValue == true)
            {
                var status = filtro.Concluido.Value
                    ? TipoStatusMovimentacao.Concluido
                    : TipoStatusMovimentacao.Pendente;

                queryable = queryable.Where(x => x.Status == status);
            }

            if (filtro?.TipoMovimentacao.HasValue == true)
            {
                var tipoMovimentacao = filtro.TipoMovimentacao.Value;

                queryable = queryable.Where(x => x.Tipo == tipoMovimentacao);
            }

            return await queryable.ToListAsync();
        }
    }
}
