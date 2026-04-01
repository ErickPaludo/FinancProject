using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Convites_.Commands;
using Financ.Application.DTOs.Base;
using Financ.Application.DTOs.Convites.Get;
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
    public class CriaConviteHandler : IRequestHandler<CriaConviteCommand, Resultado<BasePost<GetCriaConviteDTO>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CriaConviteHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Resultado<BasePost<GetCriaConviteDTO>>> Handle(CriaConviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Usuario usuarioDestinatario = await _unitOfWork.usuariosRepostorio.BuscarObjetoUnico(x => x.Email.Equals(request.emailDestinatario!));

                if (usuarioDestinatario is null)
                    return Resultado<BasePost<GetCriaConviteDTO>>.GeraFalha(Falha.NaoEncontrado("Usuário destinatário não encontrado."));

                var conta = await _unitOfWork.contasRepositorio.BuscarContaComUsuarios(x => x.Id == request.idConta && x.ContaUsuarios.Any(u => u.IdUsuario == request.idRemetente));

                if (conta is null)
                    return Resultado<BasePost<GetCriaConviteDTO>>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));

                ContasUsuarios? contaUsuarioRemetente = conta.ContaUsuarios.FirstOrDefault(x => x.IdUsuario == request.idRemetente);

                Convites convite = new Convites(request.acesso, contaUsuarioRemetente, usuarioDestinatario,request.expiracaoContaUsuario);
                await _unitOfWork.convitesRepostorio.Adicionar(convite);
                await _unitOfWork.Commit();

                return Resultado<BasePost<GetCriaConviteDTO>>.GeraSucesso(ConvitesMapper.ParaDTO(convite));
            }
            catch (ConvitesValidacao ex)
            {
                return Resultado<BasePost<GetCriaConviteDTO>>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
