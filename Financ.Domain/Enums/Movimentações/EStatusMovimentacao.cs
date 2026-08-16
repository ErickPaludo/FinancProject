using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Enums.Movimentações
{
    public enum EStatusMovimentacao
    {
        Pendente,
        Concluida,
        Excluido,
        Oculta //Exclusivo para movimentação fixa base
    }
}
