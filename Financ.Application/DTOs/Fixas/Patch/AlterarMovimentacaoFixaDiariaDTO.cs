using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Fixas.Patch
{
    public record AlterarMovimentacaoFixaDiariaDTO(StatusMovimentacaoFixa? Status, DateOnly? DataInicio, DateOnly? DataFim, int[]? OcorrenciaDiaria);
}
