using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums.Movimentações;
using Financ.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Financ.Application.CQRS.Movimentação.Handlers
{
    public class DashMovimentacoesTotaisHandler : IRequestHandler<DashMovimentacoesTotaisCommand, Resultado<BaseGet<DashboardTotalContasDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashMovimentacoesTotaisHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BaseGet<DashboardTotalContasDTO>>> Handle(DashMovimentacoesTotaisCommand request, CancellationToken cancellationToken)
        {
            Conta? conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.IdConta);

            var queryable = _unitOfWork.movimentacaoRepositorio.BuscaMovimentacaoComContasUsuarios();

            queryable = queryable.Where(x => x.IdConta == request.IdConta);
            var mov = await queryable.ToListAsync();

            var agrupamentoPorMes = mov.GroupBy(mov => new { mov.DthrMovimentacao.Month,mov.DthrMovimentacao.Year })
                .Select(g => 
                new DashAgrupadoPorMes(
                    g.Key.Year,
                    g.Key.Month, 
                    new GrupoMovDashTotalDTO(
                        g.Where(x => x.Tipo == TipoMovimentacao.Entrada && x.Status == TipoStatusMovimentacao.Concluido)
                        .Sum(mov => mov.Valor),
                        g.Where(x => x.Tipo == TipoMovimentacao.Saida && x.Status == TipoStatusMovimentacao.Concluido)
                        .Sum(mov => mov.Valor)
                        )))
                .OrderBy(x => x.Ano).ThenBy(x => x.Mes).ToList();

            return Resultado<BaseGet<DashboardTotalContasDTO>>.GeraSucesso(new BaseGet<DashboardTotalContasDTO>(new DashboardTotalContasDTO(ContaMapper.ParaDTO(conta!),agrupamentoPorMes)));
        }
    }
}
