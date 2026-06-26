using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Convites.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.ContasUsuarios.Get;
using Financ.Application.DTOs.ContasUsuarios.Post;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Entidades.ContasBancarias;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Validacoes.ContasBancarias;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Convites.Handler
{
    public class AceitaConviteHandler : IRequestHandler<AceitaConviteCommand, Resultado<BasePost<RetornaPostCadastroDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AceitaConviteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<RetornaPostCadastroDTO>>> Handle(AceitaConviteCommand request, CancellationToken cancellationToken)
        {
                var convite = await _unitOfWork.convitesRepostorio.BuscarConviteComContasEContasUsuarios(x => x.Id == request.IdConvite && x.IdUsuarioDestinatario.Equals(request.IdUsuario));

                if (convite is null)
                    return Resultado<BasePost<RetornaPostCadastroDTO>>.GeraFalha(Falha.NaoEncontrado("Convite não encontrado!"));

                convite.AceitaConvite(request.aceito);

                if (!request.aceito)
                {
                    _unitOfWork.convitesRepostorio.Atualiza(convite);
                    await _unitOfWork.Commit();
                    return Resultado<BasePost<RetornaPostCadastroDTO>>.GeraSucesso(new BasePost<RetornaPostCadastroDTO>(new RetornaPostCadastroDTO(request.aceito, null, convite.Observacao)));
                }

                ContaUsuario? contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarObjetoUnico(x => x.IdConta == convite.IdConta && x.IdUsuario.Equals(request.IdUsuario));
                if (contaUsuario is null)
                {
                    contaUsuario = new ContaUsuario(convite);
                    contaUsuario = await _unitOfWork.contasUsuariosRepositorio.Adicionar(contaUsuario);
                }
                else
                {
                    contaUsuario.RetornaParaConta(convite);
                    _unitOfWork.contasUsuariosRepositorio.Atualiza(contaUsuario);
                }


                _unitOfWork.convitesRepostorio.Atualiza(convite);

                await _unitOfWork.Commit();
                return Resultado<BasePost<RetornaPostCadastroDTO>>.GeraSucesso(ContaUsuarioMapper.ParaDTO(convite, contaUsuario));
        }
    }
}
