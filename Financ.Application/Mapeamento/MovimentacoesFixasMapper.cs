using Financ.Application.DTOs.Movimentações.Fixas.Get;
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
                movimentacoesFixas.Add(
                    new GetMovimentacaoFixaDTO(
                        mf.Id,
                        mf.Tipo,
                        mf.DataInicio,
                        mf.DataFim,
                        mf.DataOcorrencia,
                        mf.DiasFixosDiarios is not null ? mf.DiasFixosDiarios.Select(x => x.DiaSemana).ToArray() : null,
                         DateOnly.FromDateTime(DateTime.UtcNow) > mf.DataFim,
                        MovimentacaoMapper.ParaDTO(mf.Movimentacao)
                        ) );
            });


            return movimentacoesFixas;
        }
    }
}
