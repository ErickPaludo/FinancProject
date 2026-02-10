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
using Financ.Application.Interfaces;
using Financ.Application.Mapeamento;

namespace Financ.Application.CQRS.Handler
{
    public class RetornaConvitesHandler : IRequestHandler<RetornaConvitesQuery, Resultado<List<GetRetornaConvitesDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConvitesLeituraRepositorio _convitesLeituraRepositorio;
        private readonly IUsuariosIdentityServicos _usuarioIdentity;

        public RetornaConvitesHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioIdentity, IConvitesLeituraRepositorio convitesLeituraRepositorio)
        {
            _unitOfWork = unitOfWork;
            _usuarioIdentity = usuarioIdentity;
            _convitesLeituraRepositorio = convitesLeituraRepositorio;
        }

        public async Task<Resultado<List<GetRetornaConvitesDTO>>> Handle(RetornaConvitesQuery request, CancellationToken cancellationToken)
        {
            var convites = await _convitesLeituraRepositorio.RetornoConvites(request.IdUsuario, request.RetornaConvitesRemetente);

            var convitesDTO = ConvitesMapper.ParaDTO(convites);

            if (convitesDTO.Count() == 0)
                return Resultado<List<GetRetornaConvitesDTO>>.GeraFalha(Falha.NaoEncontrado("Nenhum convite foi encontrado!"));

            return Resultado<List<GetRetornaConvitesDTO>>.GeraSucesso(convitesDTO);
        }
    }
}
