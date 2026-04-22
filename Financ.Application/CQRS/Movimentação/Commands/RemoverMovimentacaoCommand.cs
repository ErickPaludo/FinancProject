using Financ.Application.Comun.Resultado;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Commands
{
    public record RemoverMovimentacaoCommand(string idUsuario, int idMovimentacao) : IRequest<Resultado<string>>;
}
