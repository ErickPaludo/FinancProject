using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Usuarios.Querys;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Application.Mapeamento;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Usuarios.Handler
{
    public class RetornaUsuarioHandler : IRequestHandler<RetornaUsuarioPorIdQuery, Resultado<RetornaUsuarioDTO>>
    {

        public async Task<Resultado<RetornaUsuarioDTO>> Handle(RetornaUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {

            return Resultado<RetornaUsuarioDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado."));

        }
    }
}
