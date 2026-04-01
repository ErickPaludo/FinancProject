using Financ.Application.Comun.Resultado;
using Financ.Application.DTOs.Convites.Get;
using Financ.Domain.Interfaces;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Financ.Application.Mapeamento;
using Financ.Application.CQRS.Convites_.Query;
using Financ.Domain.Entidades;
using Financ.Application.DTOs.Base;

namespace Financ.Application.CQRS.Convites_.Handler
{
    public class RetornaConvitesHandler : IRequestHandler<RetornaConvitesQuery, Resultado<BaseGet<GetRetornaConvitesDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaConvitesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGet<GetRetornaConvitesDTO>>> Handle(RetornaConvitesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Convites?> convites = await _unitOfWork.convitesRepostorio.ObterConviteComRemetenteDestinatarioEContaAsync(request.RetornaConvitesRemetente ? r => r.IdUsuarioRemetente.Equals(request.IdUsuario) : d => d.IdUsuarioDestinatario.Equals(request.IdUsuario));

            if(!convites.Any())
                return Resultado<BaseGet<GetRetornaConvitesDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhum convite foi encontrado!"));

             return Resultado<BaseGet<GetRetornaConvitesDTO>>.GeraSucesso(ConvitesMapper.ParaDTO(convites, request.RetornaConvitesRemetente));
        }
    }
}
