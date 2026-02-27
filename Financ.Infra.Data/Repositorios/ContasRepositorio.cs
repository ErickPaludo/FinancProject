using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
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
    public class ContasRepositorio : BaseRepositorio<Conta> , IContasRepositorio
    {
        private AppContextoData _contexto;
        public ContasRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public Task<Conta> BuscarContaComUsuarios(Expression<Func<Conta, bool>> predicado)
        {
            return _contexto.Contas.Include(c => c.ContaUsuarios).FirstOrDefaultAsync(predicado);
        }
    }
}
