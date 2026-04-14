using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Convites.Get;
using Financ.Domain.Enums.ContasBancarias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites.Commands
{
    public record CriaConviteCommand(string idRemetente,string emailDestinatario,int idConta,TiposAcessos acesso, int? expiracaoContaUsuario) : IRequest<Resultado<BasePost<GetCriaConviteDTO>>>;
}
