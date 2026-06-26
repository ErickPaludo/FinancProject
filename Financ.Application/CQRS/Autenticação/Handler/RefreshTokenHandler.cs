using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Autenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces.Autenticação;
using Financ.Domain.Entidades.Segurança;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.Segurança;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Autenticação.Handler
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Resultado<RetornaTokenDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAutenticacaoServico _tokenService;
        public RefreshTokenHandler(IUnitOfWork unitOfWork, IAutenticacaoServico tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }
        public async Task<Resultado<RetornaTokenDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
           
                Autenticacao? auth = await _unitOfWork.autenticacoesRepositorio.BuscarAuthComUsuarios(x => x.RefreshToken!.Equals(request.refreshToken));

                if (auth is null)
                    return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoAutorizado("Refresh token invalido"));

                auth.ValidaRefreshToken(request.refreshToken);

                var refreshToken = _tokenService.RefreshToken(auth!, request.refreshToken);

                auth.AtualizaRefreshToken(refreshToken.refreshToken, refreshToken.expirationRefreshToken);
                _unitOfWork.autenticacoesRepositorio.Atualiza(auth);
                await _unitOfWork.Commit();
                return Resultado<RetornaTokenDTO>.GeraSucesso(new RetornaTokenDTO
                {
                    Expiracao = refreshToken.expirationTokenFormatado,
                    ExpiracaoRefresh = refreshToken.expirationRefreshTokenFormatado,
                    RefreshToken = refreshToken.refreshToken,
                    Token = refreshToken.token,
                });
           
        }
    }
}
