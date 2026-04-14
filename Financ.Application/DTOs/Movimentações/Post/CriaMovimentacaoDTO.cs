using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Post
{
    public record class CriaMovimentacaoDTO(int idConta, int? idCategoria,TipoMovimentacao tipo, decimal valor, bool concluido, string titulo, string? observacao, DateTime? dthrMovimentacao, DateTime? dthrConclusao);
}
