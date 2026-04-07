using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums;
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
    public class ContasRepositorio : BaseRepositorio<Conta> , IContasRepositorio
    {
        private AppContextoData _contexto;
        public ContasRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public Task<Conta?> BuscarContaComUsuarios(Expression<Func<Conta, bool>> predicado)
        {
            return _contexto.Contas.Include(c => c.ContaUsuarios).Include(c => c.Convites.Where(e => !e.Aceito.HasValue && e.Expiracao > DateTime.UtcNow)).FirstOrDefaultAsync(predicado);
        }
    }
}
