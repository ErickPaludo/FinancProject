using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Interfaces.Repositorios.Movimentações;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Movimentações
{
    public class MovimentacaoRepositorio : BaseRepositorio<Movimentacao>, IMovimentacaoRepositorio
    {
        public MovimentacaoRepositorio(AppContextoData contexto) : base(contexto)
        {
        }
    }
}
