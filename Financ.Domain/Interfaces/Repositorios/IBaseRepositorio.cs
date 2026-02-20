using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces
{
    public interface IBaseRepositorio<T> where T : class
    {
        Task<T> Adicionar(T entity);
        Task<T?> BuscarPeloId<TId>(TId? id);
        Task<bool> ExisteId(Expression<Func<T, bool>> predicado);
        Task<T?> BuscarObjetoUnico(Expression<Func<T, bool>> predicado);
        Task<IEnumerable<T>> BuscarPorCondicao(Expression<Func<T, bool>> predicado);
        void Atualiza(T entity);
        void Delete(T entity);
    }
}
