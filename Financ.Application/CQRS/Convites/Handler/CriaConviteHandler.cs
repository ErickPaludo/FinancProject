using Financ.Application.Comun.Resultado;
using Financ.Application.CQRS.Commands;
using Financ.Application.DTOs.Convites.Get;
using Financ.Application.Mapeamento;
using Financ.Domain.Entidades;
using Financ.Domain.Enums;
using Financ.Domain.Interfaces;
using Financ.Domain.Interfaces.Autenticação;
using Financ.Domain.Interfaces.InterfaceEntidades;
using Financ.Domain.Validacoes;
using NetDevPack.SimpleMediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financ.Application.CQRS.Handler
{
    public class CriaConviteHandler : IRequestHandler<CriaConviteCommand, Resultado<GetCriaConviteDTO>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUsuariosIdentityServicos _usuarioIdentity;
        public CriaConviteHandler(IUnitOfWork unitOfWork, IUsuariosIdentityServicos usuarioIdentity)
        {
            _unitOfWork = unitOfWork;
            _usuarioIdentity = usuarioIdentity;
        }
        public async Task<Resultado<GetCriaConviteDTO>> Handle(CriaConviteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var idUsuarioDestinatario = await _usuarioIdentity.ObtemIdUsuario(request.emailDestinatario!);
                if (string.IsNullOrEmpty(idUsuarioDestinatario))
                       return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Usuário destinatário do convite não existe."));
                
                var conta = await _unitOfWork.contasRepositorio.BuscarPorCondicao(x => x.Id == request.idConta);

                if (conta is null)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Conta não encontrada."));
                
                var contaUsuario = await _unitOfWork.contasUsuariosRepositorio.BuscarPorCondicao(x => x.IdConta == request.idConta );
               
                bool usuariosMaster = contaUsuario.Where(x => x.Acesso == TiposAcessos.Mestre).Take(2).Count() == 2;
                if (usuariosMaster)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("A conta já possui 2 usuários masters."));

                var contaUsuarioRemetente = contaUsuario.FirstOrDefault(x => x.IdUsuario == request.idRemetente);
                if (contaUsuarioRemetente is null)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.NaoEncontrado("Você não está associado a está conta."));
                
                var contaUsuarioDestinatario = contaUsuario.FirstOrDefault(x => x.IdUsuario == idUsuarioDestinatario);

                bool usuarioAssociado = contaUsuario.Any(x => x.IdUsuario == idUsuarioDestinatario);
                if (usuarioAssociado)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional("Usuário já pertence a está conta"));  

                bool usuarioPossuiConvite = (await _unitOfWork.convitesRepostorio.BuscarPorCondicao(
                x => x.IdConta == request.idConta &&
                x.IdUsuarioDestinatario == idUsuarioDestinatario &&
                x.IdUsuarioRemetente == request.idRemetente &&
                DateTime.Now <= x.Expiracao &&
                x.Aceito == null)).Any();
                

                if(usuarioPossuiConvite)
                    return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional("Já existe um convite em andamento."));

                Usuario usuarioDestinatario = await _usuarioIdentity.ObtemUsuario(idUsuarioDestinatario);
                Usuario usuarioRemetente = await _usuarioIdentity.ObtemUsuario(request.idRemetente);

                Convites convite = new Convites(contaUsuarioRemetente!.Contas!,request.acesso, contaUsuarioRemetente, usuarioDestinatario);
                await _unitOfWork.convitesRepostorio.Adicionar(convite);
                await _unitOfWork.Commit();

                return Resultado<GetCriaConviteDTO>.GeraSucesso(ConvitesMapper.ParaDTO(convite));
            }
            catch (ConvitesValidacao ex)
            {
                return Resultado<GetCriaConviteDTO>.GeraFalha(Falha.ErroOperacional(ex.Message));
            }
        }
    }
}
