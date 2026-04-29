using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.ContasUsuarios.Commands
{
    public record FavoritaContaUsuarioCommand(int IdConta, string IdUsuario) : IRequest<Resultado<BaseGet<RetornaContasDTO>>>;
}
