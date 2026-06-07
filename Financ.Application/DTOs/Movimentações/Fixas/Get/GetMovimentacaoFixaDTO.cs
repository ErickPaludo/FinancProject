using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Entidades.Movimentações.Fixas;
using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Fixas.Get
{
    public record GetMovimentacaoFixaDTO(int Id, TipoMovimentacaoFixa Tipo, DateOnly DataInicio, DateOnly DataFim, DateTime? DataOcorrencia, int[]? OcorrenciaDiaria,bool Expirado,MovimentacaoDTO MovimentacaoBase);
}
