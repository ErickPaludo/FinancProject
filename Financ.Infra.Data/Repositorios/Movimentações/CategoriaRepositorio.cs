using Financ.Domain.Entidades.Movimentações;
using Financ.Domain.Interfaces.Repositorios.Movimentações;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Movimentações
{
    public class CategoriaRepositorio : BaseRepositorio<Categoria>, ICategoriaRepositorio
    {
        private readonly AppContextoData _contexto;
        public CategoriaRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<Categoria?> ObterCategoriaComConta(Expression<Func<Categoria, bool>> predicado)
        {
           return await _contexto.Categoria
                .Include(c => c.Conta)
                .ThenInclude(u => u.ContaUsuarios)
                .Where(predicado).FirstOrDefaultAsync();
        }
    }
}
