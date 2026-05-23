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
using Financ.Application.DTOs.Base;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Application.CQRS.Convites.Query;

namespace Financ.Application.CQRS.Convites.Handler
{
    public class RetornaConvitesHandler : IRequestHandler<RetornaConvitesQuery, Resultado<BaseGetList<GetRetornaConvitesDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RetornaConvitesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Resultado<BaseGetList<GetRetornaConvitesDTO>>> Handle(RetornaConvitesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Convite>? convites = await _unitOfWork.convitesRepostorio.ObterConviteComRemetenteDestinatarioEContaAsync(request.RetornaConvitesRemetente ? r => r.IdUsuarioRemetente.Equals(request.IdUsuario) : d => d.IdUsuarioDestinatario.Equals(request.IdUsuario));

            if(!convites.Any())
                return Resultado<BaseGetList<GetRetornaConvitesDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhum convite foi encontrado!"));

             return Resultado<BaseGetList<GetRetornaConvitesDTO>>.GeraSucesso(ConviteMapper.ParaDTO(convites.OrderByDescending(c => c.Expiracao).ThenBy(c => c.Aceito), request.RetornaConvitesRemetente));
        }
    }
}
