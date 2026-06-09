using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Fixas.Patch
{
    public record AlteraMovimentacaoFixaDTO(TipoMovimentacaoFixa? Tipo, StatusMovimentacaoFixa? Status, DateOnly? DataInicio, DateOnly? DataFim, DateTime? DataOcorrencia);
}
