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

namespace Financ.Application.CQRS.Convites_.Handler
{
    public class RetornaConvitesHandler : IRequestHandler<RetornaConvitesQuery, Resultado<List<GetRetornaConvitesDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaConvitesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<List<GetRetornaConvitesDTO>>> Handle(RetornaConvitesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Convites?> convites = await _unitOfWork.convitesRepostorio.ObterConviteComRemetenteDestinatarioEContaAsync(request.RetornaConvitesRemetente ? r => r.IdUsuarioRemetente.Equals(request.IdUsuario) : d => d.IdUsuarioDestinatario.Equals(request.IdUsuario));

            if(!convites.Any())
                return Resultado<List<GetRetornaConvitesDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhum convite foi encontrado!"));

             return Resultado<List<GetRetornaConvitesDTO>>.GeraSucesso(ConvitesMapper.ParaDTO(convites));
        }
    }
}
