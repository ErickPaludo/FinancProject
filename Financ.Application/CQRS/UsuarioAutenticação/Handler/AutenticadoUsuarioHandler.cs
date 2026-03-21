using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using NetDevPack.SimpleMediator;
using Newtonsoft.Json.Linq;
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
        private readonly ITokenService _token;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPassService _passService;

        public AutenticadoUsuarioHandler(IAutenticacao autenticacao,ITokenService token,IUnitOfWork unitOfWork,IPassService passService)
        {
            _autenticacao = autenticacao;
            _token = token;
            _unitOfWork = unitOfWork;
            _passService = passService;
        }

        public async Task<Resultado<RetornaTokenDTO>> Handle(AutenticadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Email.Equals(request.Email));

            if(usuario is null)
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado!"));

            if(!_passService.ValidaPassArgon(usuario.HashPass, request.Senha, usuario.Salt))
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoAutorizado("Senha inválida."));

            var token = _token.GeraToken(usuario.Id, request.Email);

            usuario.AtualizaRefreshToken(token.RefreshToken, Utilitarios.DateTimeInUnixTimestamp(token.ExpiracaoRefresh));

             _unitOfWork.usuariosRepostorio.Atualiza(usuario);

            await _unitOfWork.Commit();

            return Resultado<RetornaTokenDTO>.GeraSucesso(token);
        }
    }
}
