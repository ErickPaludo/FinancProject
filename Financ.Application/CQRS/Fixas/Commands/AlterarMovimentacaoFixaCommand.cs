using Financ.Application.Comun.Resultado;
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
    public record AlterarMovimentacaoFixaCommand(int IdConta,int IdFixa,string IdUsuario, TipoMovimentacaoFixa? Tipo, StatusMovimentacaoFixa? Status, DateOnly? DataInicio, DateOnly? DataFim, DateTime? DataOcorrencia, bool Diario = false) : IRequest<Resultado<GetMovimentacaoFixaDTO>>;
}
