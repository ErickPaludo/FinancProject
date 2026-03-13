using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Convites.Get;
using Financ.Domain.Enums;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites_.Commands
{
    public record CriaConviteCommand(string idRemetente,string emailDestinatario,int idConta,TiposAcessos acesso) : IRequest<Resultado<GetCriaConviteDTO>>;
}
