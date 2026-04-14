using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.DTOs.ContasUsuarios.Get
{
    public sealed record RetornaContaUsuarioDTO(int IdContaUsuario, string idUsuario, string email, string primeiroNome, string segundoNome, string nomeCompleto);
}
