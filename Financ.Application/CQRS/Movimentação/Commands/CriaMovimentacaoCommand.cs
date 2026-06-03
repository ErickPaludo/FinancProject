using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands.Base;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Enums.Movimentações;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Commands
{
    public record CriaMovimentacaoCommand : MovimentacaoBaseCommand, IRequest<Resultado<BasePost<MovimentacaoDTO>>>
    {
        public bool concluido { get; init; }
        public DateTime? dthrMovimentacao { get; init; }
        public DateTime? dthrConclusao { get; init; }
    }
}

