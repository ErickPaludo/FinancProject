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
    public class AutenticacoesRepositorio : BaseRepositorio<Autenticacao>, IAutenticacoesRepositorio
    {
        private readonly AppContextoData _contexto;
        public AutenticacoesRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public Task<Autenticacao?> BuscarAuthComUsuarios(Expression<Func<Autenticacao, bool>> predicado)
        {
            return _contexto.Autenticacao.Include(c => c.Usuario).FirstOrDefaultAsync(predicado);
        }
    }
}
