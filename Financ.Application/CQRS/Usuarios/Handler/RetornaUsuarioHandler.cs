using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Usuarios.Querys;
using Financ.Application.DTOs.Usuarios.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Interfaces.Autenticação;
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
        private readonly IUsuariosIdentityServicos _usuariosServico;
        public RetornaUsuarioHandler(IUsuariosIdentityServicos usuariosServico) => _usuariosServico = usuariosServico;

        public async Task<Resultado<RetornaUsuarioDTO>> Handle(RetornaUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            var usuario = await _usuariosServico.ObtemUsuario(request.IdUsuario);

            if (usuario is null)
                return Resultado<RetornaUsuarioDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado."));

            return Resultado<RetornaUsuarioDTO>.GeraSucesso(UsuarioMapper.ParaDTO(usuario));

        }
    }
}
