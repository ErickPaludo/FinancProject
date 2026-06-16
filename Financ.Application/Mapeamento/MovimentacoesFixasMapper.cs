using Financ.Application.DTOs.Fixas.Get;
using Financ.Domain.Entidades.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Mapeamento
{
    public static class MovimentacoesFixasMapper
    {
        public static List<GetMovimentacaoFixaDTO> ParaDTO(List<MovimentacaoFixa> movimentacaoFixa)
        {
            List<GetMovimentacaoFixaDTO> movimentacoesFixas = new ();

            movimentacaoFixa.ForEach(mf =>
            {
                movimentacoesFixas.Add(ParaDTO(mf));
            });
            return movimentacoesFixas;
        }

        public static GetMovimentacaoFixaDTO ParaDTO(MovimentacaoFixa movimentacaoFixa) 
            => new GetMovimentacaoFixaDTO(
                movimentacaoFixa.Id,
                movimentacaoFixa.Tipo,
                movimentacaoFixa.DataInicio,
                movimentacaoFixa.DataFim,
                movimentacaoFixa.DataOcorrencia,
                movimentacaoFixa.DiasFixosDiarios.Count > 0 ? movimentacaoFixa.DiasFixosDiarios.Select(x => x.DiaSemana).ToArray() : null,
                DateTime.UtcNow > movimentacaoFixa.DataFim,
                MovimentacaoMapper.ParaDTO(movimentacaoFixa.Movimentacao));
    }
}
