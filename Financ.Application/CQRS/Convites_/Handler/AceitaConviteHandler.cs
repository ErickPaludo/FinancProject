using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Convites_.Commands;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites_.Handler
{
    public class AceitaConviteHandler : IRequestHandler<AceitaConviteCommand, Resultado<RetornaPostCadastroDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AceitaConviteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<RetornaPostCadastroDTO>> Handle(AceitaConviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var convite = await _unitOfWork.convitesRepostorio.BuscarConviteComContasEContasUsuarios(x => x.Id == request.IdConvite && x.IdUsuarioDestinatario.Equals(request.IdUsuario));

                if (convite is null)
                    return Resultado<RetornaPostCadastroDTO>.GeraFalha(Falha.NaoEncontrado("Convite não encontrado!"));

                convite.AceitaConvite(request.aceito);

                if (!request.aceito)
                {
                    _unitOfWork.convitesRepostorio.Atualiza(convite);
                    await _unitOfWork.Commit();
                    return Resultado<RetornaPostCadastroDTO>.GeraSucesso(new RetornaPostCadastroDTO(request.aceito, null,convite.Observacao));
                }

                var contaUsuario = new ContasUsuarios(convite);

                contaUsuario = await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario);

                _unitOfWork.convitesRepostorio.Atualiza(convite);

                await _unitOfWork.Commit();
                return Resultado<RetornaPostCadastroDTO>.GeraSucesso(ContasUsuariosMapper.ParaDTO(convite, contaUsuario));
            }
            catch (ContasUsuariosValidacao contasUsuariosExcessao)
            {
                return Resultado<RetornaPostCadastroDTO>.GeraFalha(Falha.ErroOperacional(contasUsuariosExcessao.Message));
            }
        }
    }
}
