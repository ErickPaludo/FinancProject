
using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces
{
    public interface IContasRepositorio : IBaseRepositorio<Conta>{
        Task<Conta> BuscarContaComUsuarios(Expression<Func<Conta, bool>> predicado);
    }
}
