using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Movimentações.Fixas.Get;
using Financ.Domain.Enums.Movimentações.Fixas;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Fixas.Commands
{
    public record RetornarMovimentacoesFixasCommand(int IdConta,string IdUsuario,TipoMovimentacaoFixa? Tipo) : IRequest<Resultado<BaseGetList<GetMovimentacaoFixaDTO>>>;
}
