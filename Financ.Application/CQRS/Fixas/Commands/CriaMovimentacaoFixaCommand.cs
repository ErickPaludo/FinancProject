using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Movimentação.Commands.Base;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using Financ.Domain.Enums.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Commands
{
    public record CriaMovimentacaoFixaCommand : MovimentacaoBaseCommand, IRequest<Resultado<BasePost<string>>>
    {
        public TipoMovimentacaoFixa TipoFixo { get; set; }
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public DateTime DataOcorrencia { get; set; }
    }
}
