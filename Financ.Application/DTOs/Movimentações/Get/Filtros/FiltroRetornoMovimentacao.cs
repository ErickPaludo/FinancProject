using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get.Filtros
{
    public sealed record FiltroRetornoMovimentacao(int? IdMovimentacao,bool? Concluido,TipoMovimentacao? TipoMovimentacao);
}
