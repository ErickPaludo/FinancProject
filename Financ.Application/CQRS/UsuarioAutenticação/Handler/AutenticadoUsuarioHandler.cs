using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;
using Financ.Application.Services;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
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
        private readonly ITokenService _token;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPassService _passService;

        public AutenticadoUsuarioHandler(ITokenService token, IUnitOfWork unitOfWork, IPassService passService)
        {
            _token = token;
            _unitOfWork = unitOfWork;
            _passService = passService;
        }

        public async Task<Resultado<RetornaTokenDTO>> Handle(AutenticadoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Email.Equals(request.Email));

            if (usuario is null)
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado!"));

            if (!_passService.ValidaPassArgon(usuario.HashPass, request.Senha, usuario.Salt))
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoAutorizado("Senha inválida."));


            var autenticacao = await _unitOfWork.autenticacoesRepositorio.BuscarAuthComUsuarios(x => x.Usuario.Id.Equals(usuario.Id));
            var tokenJwt = _token.GeraToken(usuario.Id, request.Email);

            if (autenticacao is null)
            {
                Autenticacao authJwt = new Autenticacao(usuario.Id, tokenJwt.RefreshToken, Utilitarios.DateTimeInUnixTimestamp(tokenJwt.ExpiracaoRefresh));
               await _unitOfWork.autenticacoesRepositorio.Adicionar(authJwt);
            }
            else
            {
                autenticacao.AtualizaRefreshToken(tokenJwt.RefreshToken, Utilitarios.DateTimeInUnixTimestamp(tokenJwt.ExpiracaoRefresh));
                _unitOfWork.autenticacoesRepositorio.Atualiza(autenticacao);
            }

            await _unitOfWork.Commit();

            return Resultado<RetornaTokenDTO>.GeraSucesso(tokenJwt);
        }
    }
}
