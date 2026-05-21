using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces.Repositorios.ContasBancarias;
using Financ.Infra.Data.Contexto;
using Financ.Infra.Data.Repositorios.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios.ContasBancarias
{
    public class ConvitesRepositorio : BaseRepositorio<Convite>, IConvitesRepostorio
    {
        private AppContextoData _contexto;

        public ConvitesRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<Convite?> BuscarConviteComContasEContasUsuarios(Expression<Func<Convite, bool>> predicado)
        {
            return await _contexto.Convites.Include(c => c.Conta)
                .ThenInclude(c => c.ContaUsuarios)
                .Where(x => x.Expiracao >= DateTime.UtcNow).FirstOrDefaultAsync(predicado);
        }

        public async Task<IEnumerable<Convite>> ObterConviteComRemetenteDestinatarioEContaAsync(Expression<Func<Convite, bool>> predicado)
        {
            return await _contexto.Convites.Include(c => c.Conta)
                           .Include(u => u.Remetente)
                           .Include(u => u.Destinatario)
                           .Where(x =>  x.Expiracao >= DateTime.UtcNow).Where(predicado).ToListAsync();
        }
    }
}
