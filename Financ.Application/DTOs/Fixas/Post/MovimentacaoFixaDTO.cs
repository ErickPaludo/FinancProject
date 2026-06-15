using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Fixas.Post
{
    public record MovimentacaoFixaDTO(TipoMovimentacao tipo, decimal valor, string titulo, string? observacao, int[]? IdsCategoria);
}
