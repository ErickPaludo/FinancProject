using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Commands
{
    public record MaterializaMovimentacaoFixaCommand(int IdMovimentacao, string IdUsuario,DateTime DataMovimentacao) : IRequest<Resultado<BasePost<MovimentacaoDTO>>>;
}
