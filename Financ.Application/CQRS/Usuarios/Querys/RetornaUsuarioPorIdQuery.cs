using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Usuarios.Get;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Usuarios.Querys
{
    public class RetornaUsuarioPorIdQuery : IRequest<Resultado<RetornaUsuarioDTO>>
    {
        public string IdUsuario { get; private set; }
        public RetornaUsuarioPorIdQuery(string idUsuario)
        {
            IdUsuario = idUsuario;
        }
    }
}
