using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Interfaces.Autenticação;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.UsuarioAutenticação.Handler
{
    public class AutenticadoUsuarioHandler : IRequestHandler<AutenticadoUsuarioCommand, Resultado<RetornaTokenDTO>>
    {
        private readonly IAutenticacao _autenticacao;
        private readonly IUsuariosIdentityServicos _usuarioServicos;
        public AutenticadoUsuarioHandler(IAutenticacao autenticacao, IUsuariosIdentityServicos usuarioServicos)
        {
            _autenticacao = autenticacao;
            _usuarioServicos = usuarioServicos;
        }

        public async Task<Resultado<RetornaTokenDTO>> Handle(AutenticadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            bool autenticador = await _autenticacao.Autenticador(request.Email, request.Senha);
            if (autenticador)
            {
                var token = _autenticacao.GeraToken(await _usuarioServicos.ObtemIdUsuario(request.Email), request.Email);
                return Resultado<RetornaTokenDTO>.GeraSucesso(AutenticacaoMapper.ParaDTO(token.email,token.Expiracao));
            }
            return Resultado<RetornaTokenDTO>.GeraFalha(Falha.ErroOperacional("Usuário ou senha inválidos!"));
        }
    }
}
