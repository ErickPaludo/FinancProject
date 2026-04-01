using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Autenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces.Autenticação;
using Financ.Application.Interfaces.Segurança;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Autenticação.Handler
{
    public class AutenticacaoHandler : IRequestHandler<AutenticacaoCommand, Resultado<RetornaTokenDTO>>
    {
        private readonly IAutenticacaoServico _autenticacaoServico;
        private readonly ISegurancaServico _segurancaServico;
        private readonly IUnitOfWork _unitOfWork;

        public AutenticacaoHandler(IAutenticacaoServico autenticacaoServico, IUnitOfWork unitOfWork, ISegurancaServico segurancaServico)
        {
            _autenticacaoServico = autenticacaoServico;
            _unitOfWork = unitOfWork;
            _segurancaServico = segurancaServico;
        }

        public async Task<Resultado<RetornaTokenDTO>> Handle(AutenticacaoCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Email.Equals(request.Email));

            if (usuario is null)
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado!"));

            if (!_segurancaServico.ValidaSenhaArgon(usuario.HashPass, request.Senha, usuario.Salt))
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoAutorizado("Senha inválida."));


            var autenticacao = await _unitOfWork.autenticacoesRepositorio.BuscarAuthComUsuarios(x => x.Usuario.Id.Equals(usuario.Id));
            var tokenJwt = _autenticacaoServico.GeraToken(usuario.Id, request.Email);

            if (autenticacao is null)
            {
                Autenticacao novaAutenticacao = new Autenticacao(usuario.Id, tokenJwt.refreshToken, tokenJwt.expirationRefreshToken);
                await _unitOfWork.autenticacoesRepositorio.Adicionar(novaAutenticacao);
            }
            else
            {
                autenticacao.AtualizaRefreshToken(tokenJwt.refreshToken, tokenJwt.expirationRefreshToken);
                _unitOfWork.autenticacoesRepositorio.Atualiza(autenticacao);
            }

            await _unitOfWork.Commit();

            return Resultado<RetornaTokenDTO>.GeraSucesso(new RetornaTokenDTO
            {
                Expiracao = tokenJwt.expirationTokenFormatado,
                ExpiracaoRefresh = tokenJwt.expirationRefreshTokenFormatado,
                RefreshToken = tokenJwt.refreshToken,
                Token = tokenJwt.token
            });
        }
    }
}
