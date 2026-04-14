using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.Movimentações.Get
{
    public record GrupoMovimentacaoDTO(decimal Concluidos,decimal Pendentes,decimal Projetado);
}
