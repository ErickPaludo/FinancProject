using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Post
{
    public record ConcluirMovimentacaoDTO(DateTime? dthrConclusao);
}
