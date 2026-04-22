using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Patch
{
    public record AlterarMovimentacaoDTO(
        string? titulo,
        string? observacao,
        int? idCategoria,
        TipoMovimentacao? tipo,
        decimal? valor,
        DateTime? dthrMovimentacao,
        DateTime? dthrConclusao);
}

