using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Convites.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Query
{
    public class RetornaConvitesQuery : IRequest<Resultado<List<GetRetornaConvitesDTO>>>
    {
        public string IdUsuario { get; }
        public bool RetornaConvitesRemetente { get; }

        public RetornaConvitesQuery(string idUsuario, bool retornaConvitesRemetente)
        {
            IdUsuario = idUsuario;
            RetornaConvitesRemetente = retornaConvitesRemetente;
        }
    }
}
