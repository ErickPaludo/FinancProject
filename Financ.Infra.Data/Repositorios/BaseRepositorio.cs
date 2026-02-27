using Financ.Domain.Interfaces;
using Financ.Infra.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios
{
    public class BaseRepositorio<T> : IBaseRepositorio<T> where T : class
    {
        private readonly AppContextoData _contexto;
        public BaseRepositorio(AppContextoData contexto)
        {
            _contexto = contexto;
        }
        public async Task<T> Adicionar(T entity)
        {
            await _contexto.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T?> BuscarPeloId<TId>(TId? id)
        {
            return await _contexto.Set<T>().FindAsync(id);
        }

        public async Task<bool> ExisteId(Expression<Func<T,bool>> predicado)
        {
            return await _contexto.Set<T>().AnyAsync(predicado);
        }
        public async Task<T?> BuscarObjetoUnico(Expression<Func<T, bool>> predicado)
        {
            return await _contexto.Set<T>().FirstOrDefaultAsync(predicado);
        }
        public async Task<IEnumerable<T>> BuscarPorCondicao(Expression<Func<T, bool>> predicado)
        {
            return await _contexto.Set<T>().Where(predicado).ToListAsync();
        }
        public void Atualiza(T entity)
        {
          _contexto.Set<T>().Update(entity);
        }
        public void Delete(T entity)
        {
            _contexto.Set<T>().Remove(entity);
        }
    }
}
