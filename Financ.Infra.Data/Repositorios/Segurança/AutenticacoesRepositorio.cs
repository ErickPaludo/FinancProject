using Financ.Domain.Entidades.Segurança;
using Financ.Domain.Interfaces.Repositorios.Segurança;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.Segurança
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
