using Financ.Domain.Entidades;
using Financ.Domain.Interfaces.Repositorios;
using Financ.Infra.Data.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Infra.Data.Repositorios
{
    public class ConvitesRepositorio : BaseRepositorio<Convites>, IConvitesRepostorio
    {
        private AppContextoData _contexto;
        public ConvitesRepositorio(AppContextoData contexto) : base(contexto)
        {
            _contexto = contexto;
        }

        public async Task<IEnumerable<Convites>> ObtemConvites(string idUsuario)
        {
            return await _contexto.Convites
              .AsNoTracking()
              .Include(fcu => fcu.Contas)
              .Where(x => x.IdUsuarioDestinatario.Equals(idUsuario))
              .ToListAsync();
        }
    }
}
