using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands.Base;
using Financ.Application.DTOs.Base;
using Financ.Domain.Enums.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Commands
{
    public record CriaMovimentacaoFixaDiariaCommand : MovimentacaoBaseCommand, IRequest<Resultado<BasePost<string>>>
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public int[] OcorrenciasDiarias { get; set; }
    }
}
