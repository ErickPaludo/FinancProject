using Financ.Application.DTOs.Movimentações.Post;
using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Fixas.Post
{
    public record CriaMovimentacaoFixaDTO(TipoMovimentacaoFixa Tipo, DateOnly DataInicio, DateOnly DataFim, DateOnly DataOcorrencia,  CriaMovimentacaoDTO Movimentacao);
}
