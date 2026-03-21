using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.UsuarioAutenticação.Commands;
using Financ.Application.DTOs.Autenticação.Get;
using Financ.Application.Interfaces;
using Financ.Application.Services;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.UsuarioAutenticação.Handler
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Resultado<RetornaTokenDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public RefreshTokenHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }
        public async Task<Resultado<RetornaTokenDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            Usuario? usuario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.RefreshToken!.Equals(request.refreshToken));

            if (string.IsNullOrEmpty(request.refreshToken) || usuario is null || usuario.RefreshToken is null)
                return Resultado<RetornaTokenDTO>.GeraFalha(Falha.NaoAutorizado("Refresh token invalido"));

            var refreshTokenDto = _tokenService.RefreshToken(usuario!, request.refreshToken);
            usuario.AtualizaRefreshToken(refreshTokenDto.RefreshToken,Utilitarios.DateTimeInUnixTimestamp(refreshTokenDto.Expiracao));
            _unitOfWork.usuariosRepostorio.Atualiza(usuario);
            await _unitOfWork.Commit();
            return Resultado<RetornaTokenDTO>.GeraSucesso(refreshTokenDto);
        }
    }
}
