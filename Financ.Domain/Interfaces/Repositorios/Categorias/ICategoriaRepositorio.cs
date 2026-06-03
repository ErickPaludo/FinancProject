using Financ.Domain.Entidades.Categorias;
using Financ.Domain.Interfaces.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces.Repositorios.Categorias
{
    public interface ICategoriaRepositorio : IBaseRepositorio<Categoria>
    {
        Task<Categoria?> ObterCategoriaComConta(Expression<Func<Categoria, bool>> predicado);
    }
}
