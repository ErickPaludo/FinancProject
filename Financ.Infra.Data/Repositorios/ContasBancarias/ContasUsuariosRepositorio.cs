using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Entidades.Usuarios;
using Financ.Domain.Interfaces.Repositorios.ContasBancarias;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.ContasBancarias
{
    public class ContasUsuariosRepositorio : BaseRepositorio<ContaUsuario>, IContasUsuariosRepositorio
    {
        private AppContextoData _contexto;
        public ContasUsuariosRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }
        public async Task<IEnumerable<ContaUsuario>> ObterContasDoUsuario(Expression<Func<ContaUsuario, bool>> predicado)
        {
            return await _contexto.ContasUsuarios
                .AsNoTracking()
                .Include(fcu => fcu.Conta) // ISSO GERA O INNER JOIN AUTOMÁTICO
                .Where(predicado)
                .ToListAsync();
        }

        public async Task<ContaUsuario?> ObterContaUsuarioComUsuario(Expression<Func<ContaUsuario, bool>> predicado)
        {
            return await _contexto.ContasUsuarios.Include(u => u.Usuario).FirstOrDefaultAsync(predicado);
        }
    }
}
