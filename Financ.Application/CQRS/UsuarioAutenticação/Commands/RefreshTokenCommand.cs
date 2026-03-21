using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Autenticação.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.UsuarioAutenticação.Commands
{
    public record RefreshTokenCommand(string refreshToken) : IRequest<Resultado<RetornaTokenDTO>>;
}
