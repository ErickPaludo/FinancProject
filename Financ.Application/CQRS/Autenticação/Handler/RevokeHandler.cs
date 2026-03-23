using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Autenticação.Commands;
using Financ.Application.Interfaces;
using Financ.Domain.Entidades;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Autenticação.Handler
{
    public class RevokeHandler : IRequestHandler<RevokeCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public RevokeHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<string>> Handle(RevokeCommand request, CancellationToken cancellationToken)
        {
            Autenticacao? auth = await _unitOfWork.autenticacoesRepositorio.BuscarAuthComUsuarios(x => x.IdUsuario == request.idUsuario);

            if(auth is null) 
                return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Usuário não encontrado"));

            auth.RevokaToken();
            _unitOfWork.autenticacoesRepositorio.Atualiza(auth);
            await _unitOfWork.Commit();
            return Resultado<string>.GeraSucesso("");
        }
    }
}
