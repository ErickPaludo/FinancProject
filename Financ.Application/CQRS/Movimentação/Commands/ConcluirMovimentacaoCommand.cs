using Financ.Application.Comun.Resultado;
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
    public record ConcluirMovimentacaoCommand(string idUsuario,int idMovimentacao, DateTime? dthrConclusao) : IRequest<Resultado<BasePost<MovimentacaoDTO>>>;
}
