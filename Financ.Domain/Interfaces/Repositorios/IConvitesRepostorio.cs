using Financ.Domain.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces.Repositorios
{
    public interface IConvitesRepostorio : IBaseRepositorio<Convites>
    {
        Task<Convites> BuscarConviteComContasEContasUsuarios(Expression<Func<Convites, bool>> predicado);
        Task<IEnumerable<Convites>> ObterConviteComRemetenteDestinatarioEContaAsync(Expression<Func<Convites, bool>> predicado);
    }
}
