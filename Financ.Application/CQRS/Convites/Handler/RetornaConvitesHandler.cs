using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Convites.Get;
using Financ.Domain.Interfaces.Autenticação;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Application.CQRS.Query;

namespace Financ.Application.CQRS.Handler
{
    public class RetornaConvitesHandler : IRequestHandler<RetornaConvitesQuery, Resultado<List<GetRetornaConvitesDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioIdentity;

        public RetornaConvitesHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioIdentity)
        {
            _unitOfWork = unitOfWork;
            _usuarioIdentity = usuarioIdentity;
        }

        public async Task<Resultado<List<GetRetornaConvitesDTO>>> Handle(RetornaConvitesQuery request, CancellationToken cancellationToken)
        {
            var teste = await _unitOfWork.convitesRepostorio.ObtemConvites(request.IdUsuario);
            return null;
        }
    }
}
