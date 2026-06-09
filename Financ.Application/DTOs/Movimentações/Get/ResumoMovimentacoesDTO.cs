using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get
{
    public record ResumoMovimentacoesDTO(decimal SaldoReal, decimal SaldoRealizado,decimal SaldoProjetado, GrupoMovimentacaoDTO Entrada, GrupoMovimentacaoDTO Saida);
}
