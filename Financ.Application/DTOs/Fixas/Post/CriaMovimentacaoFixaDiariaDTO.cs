using Financ.Application.DTOs.Movimentações.Post;
using Financ.Domain.Enums.Movimentações.Fixas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Fixas.Post
{
    public record CriaMovimentacaoFixaDiariaDTO(DateTime DataInicio, DateTime DataFim,int[] OcorrenciaDiaria, CriaMovimentacaoDTO Movimentacao);
}
