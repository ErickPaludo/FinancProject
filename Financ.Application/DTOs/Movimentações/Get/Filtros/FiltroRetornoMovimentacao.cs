using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get.Filtros
{
    public sealed record FiltroRetornoMovimentacao(int? IdMovimentacao,bool? Concluido,string? Titulo, TipoMovimentacao? TipoMovimentacao, int[]? IdCategoria, DateTime DthrMovimentacaoInicial,DateTime DthrMovimentacaoFinal, DateTime? DthrReg, bool? RetornaFixos = false);
}
