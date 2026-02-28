using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios
{
    public class ContasUsuariosRepositorio : BaseRepositorio<ContasUsuarios>, IContasUsuariosRepositorio
    {
        private AppContextoData _contexto;
        public ContasUsuariosRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }
        public async Task<IEnumerable<ContasUsuarios>> ObterContasDoUsuario(Expression<Func<ContasUsuarios, bool>> predicado)
        {
            return await _contexto.ContasUsuarios
                .AsNoTracking()
                .Include(fcu => fcu.Conta) // ISSO GERA O INNER JOIN AUTOMÁTICO
                .Where(predicado)
                .ToListAsync();
        }
    }
}
