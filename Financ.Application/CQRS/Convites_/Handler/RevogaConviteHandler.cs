using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Convites_.Commands;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites_.Handler
{
    public class RevogaConviteHandler : IRequestHandler<RevogaConviteCommand, Resultado<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioIdentity;
        public RevogaConviteHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioIdentity)
        {
            _unitOfWork = unitOfWork;
            _usuarioIdentity = usuarioIdentity;
        }

        public async Task<Resultado<string>> Handle(RevogaConviteCommand request, CancellationToken cancellationToken)
        {
            var convite = await _unitOfWork.convitesRepostorio.BuscarConviteComConta(x => x.IdUsuarioRemetente.Equals(request.idRemetente) && x.Id == request.idConvite);

            if (convite is null)
                return Resultado<string>.GeraFalha(Falha.NaoEncontrado("Convite não encontrado!"));

            convite!.RevogaConvite(request.idRemetente);
            _unitOfWork.convitesRepostorio.Delete(convite);
            await _unitOfWork.Commit();

            return Resultado<string>.GeraSucesso("Convite revogado com sucesso!");
        }
    }
}
