using Financ.Domain.Entidades;
using Financ.Domain.Enums;
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

        public Task<Conta?> BuscarContaComUsuarios(Expression<Func<Conta, bool>> predicado)
        {
            return _contexto.Contas.Include(c => c.ContaUsuarios.Where(x => x.Status.Equals(TipoStatusContasUsuario.Ativo))).Include(c => c.Convites.Where(e => !e.Aceito.HasValue && e.Expiracao > DateTime.Now)).FirstOrDefaultAsync(predicado);
        }
    }
}
