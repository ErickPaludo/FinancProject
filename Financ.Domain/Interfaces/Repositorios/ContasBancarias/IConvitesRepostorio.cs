using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Interfaces.Repositorios.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Domain.Interfaces.Repositorios.ContasBancarias
{
    public interface IConvitesRepostorio : IBaseRepositorio<Convite>
    {
        Task<Convite> BuscarConviteComContasEContasUsuarios(Expression<Func<Convite, bool>> predicado);

        Task<IEnumerable<Convite>> ObterConviteComRemetenteDestinatarioEContaAsync(Expression<Func<Convite, bool>> predicado);
    }
}
