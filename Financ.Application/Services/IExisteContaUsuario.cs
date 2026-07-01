using Financ.Domain.Entidades.ContasBancarias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.Services
{
    public interface IExisteContaUsuario
    {
        Task<ContaUsuario> Buscar(Expression<Func<ContaUsuario, bool>> predicate);
    }
}
