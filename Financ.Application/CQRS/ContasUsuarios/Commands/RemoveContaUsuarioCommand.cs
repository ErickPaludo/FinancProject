using Financ.Application.Comun.Resultado;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Contas_Usuarios.Commands
{
    public record RemoveContaUsuarioCommand(string idUsuarioRemetente, string idUsuarioDestinatario, int idConta) : IRequest<Resultado<string>> ;
}
