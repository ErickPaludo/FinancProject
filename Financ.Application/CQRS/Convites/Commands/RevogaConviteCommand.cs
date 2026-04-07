using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.ContasUsuarios.Post;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites_.Commands
{
    public record RevogaConviteCommand(int idConvite,string idRemetente) : IRequest<Resultado<string>>;
}
