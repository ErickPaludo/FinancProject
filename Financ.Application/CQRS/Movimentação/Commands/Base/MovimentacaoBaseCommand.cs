using Financ.Domain.Enums.Movimentações;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Movimentação.Commands.Base
{
    public abstract record MovimentacaoBaseCommand
    {
        public int idConta { get; init; }
        public string idUsuario { get; init; }
        public int? idCategoria { get; init; }
        public int[]? IdsCategoria { get; init; }
        public TipoMovimentacao tipo { get; init; }
        public decimal valor { get; init; }
        public string titulo { get; init; }
        public string? observacao { get; init; }

    }
}


